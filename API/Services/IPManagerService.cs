using API.CustomExceptions;
using API.Data;
using API.DataMappers;
using API.Entities;
using API.Models;
using API.Repositories;
using API.Services.Communication;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace API.Services
{
    public class IPManagerService : IIPManagerService
    {

        private readonly IIPManagerRepository repository;
        public IPManagerService(IIPManagerRepository repository) // Dependency Injection
        {
            this.repository = repository;
        }

        // Return list of all IP details in database
        public async Task<List<IPDetails>> ListAsync()
        {
            List<IPDetails> iPDetailsList = new();
            // From DB
            foreach (IPDetailsEntity entity in await repository.ListAsync())
                iPDetailsList.Add(IPDetailsMapper.ToModel(entity));

            return iPDetailsList;
        }

        // Get IP details by IP
        public async Task<IPDetails> GetByIPAsync(string ip)
        {
            List<IPDetailsEntity> iPDetailsEntitiesList = await repository.FindByIPAsync(ip);
            if (iPDetailsEntitiesList.Any())
            {
                if (iPDetailsEntitiesList.Count == 1)  // Found one
                    return IPDetailsMapper.ToModel(iPDetailsEntitiesList.First());
                else    // Found multiple. Irregular behaviour as IP should be unique
                    throw new DuplicateIPAddressException(String.Format("Duplicate detected for IP: {0}", ip));
            }
            else
                return null;
        }

        // Add new IP details in database
        public async Task<IPDetails> SaveAsync(IPDetails details)
        {
            List<IPDetailsEntity> existingIPDetails = await repository.FindByIPAsync(details.IP);
            if (existingIPDetails.Any())
                throw new Exception("IP already exists within the database");

            await repository.AddAsync(IPDetailsMapper.ToEntity(details));

            return details;
        }

        // Update an IP details record in the database. Using IP as identifier
        public async Task<IPDetails> UpdateAsync(IPDetails details)
        {
            IPDetailsEntity entity;
            List<IPDetailsEntity> iPDetailsEntitiesList = await repository.FindByIPAsync(details.IP);
            if (iPDetailsEntitiesList.Any())
            {
                // Found one
                if (iPDetailsEntitiesList.Count == 1)
                {
                    entity = iPDetailsEntitiesList.First();
                    entity.City = details.City;
                    entity.Continent = details.Continent;
                    entity.Country = details.Country;
                    entity.Latitude = details.Latitude;
                    entity.Longitude = details.Longitude;

                    repository.UpdateAsync(entity);

                    return details;
                }
                else    // Found multiple. Irregular behaviour as IP should be unique
                    throw new DuplicateIPAddressException(String.Format("Duplicate detected for IP: {0}", details.IP));
            }
            else
                throw new Exception($"IP details are missing for IP: {details.IP}");
        }

        // Update IP details records in bulk in the database. Using IP as identifier
        public void UpdateBulk(Guid guid, IPDetails[] detailsArray, IProgress<ProgressReportModel> progress, int batchSize = 10)
        {
            // Parallel requests, but not all at the same time. Let’s do it batches for 10
            List<IPDetails> completed = new List<IPDetails>();
            int numberOfBatches = (int)Math.Ceiling((double)detailsArray.Length / batchSize);
            ProgressReportModel report = new ProgressReportModel()
            {
                Guid = guid
            };

            for (int i = 1; i <= numberOfBatches; i++)
            {
                var currentBatch = detailsArray.Skip(i * batchSize).Take(batchSize);
                var tasks = currentBatch.Select(x => UpdateAsync(x));
                Task.WhenAll(tasks);
                completed.AddRange(currentBatch.ToList());

                report.IPDetailsCompleted = completed;
                report.PercentageComplete = (i * 100) / numberOfBatches;
                progress.Report(report);

                // Used for testing purposes
                Thread.Sleep(30000); // Add 30 seconds to batch processing time
            }
        }
    }
}
