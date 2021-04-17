using API.CustomExceptions;
using API.Data;
using API.DataMappers;
using API.Entities;
using API.Repositories;
using API.Services.Communication;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<IPDetails>> ListAsync()
        {
            List<IPDetails> iPDetailsList = new();
            // From DB
            foreach (IPDetailsEntity entity in await repository.ListAsync())
                iPDetailsList.Add(IPDetailsMapper.ToModel(entity));

            return iPDetailsList;
        }

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
                throw new Exception("IP doesn't exist within the database");
        }

        public async Task<IPDetails> SaveAsync(IPDetails details)
        {
            List<IPDetailsEntity> existingIPDetails = await repository.FindByIPAsync(details.IP);
            if (existingIPDetails.Any())
                throw new Exception("IP already exists within the database");

            await repository.AddAsync(IPDetailsMapper.ToEntity(details));

            return details;
        }

        public async Task<IPDetails> UpdateAsync(IPDetails details)
        {
            List<IPDetailsEntity> iPDetailsEntitiesList = await repository.FindByIPAsync(details.IP);
            if (iPDetailsEntitiesList.Any())
            {
                // Found one
                if (iPDetailsEntitiesList.Count == 1)
                {
                    repository.UpdateAsync(IPDetailsMapper.ToEntity(details));

                    return details;
                }
                else    // Found multiple. Irregular behaviour as IP should be unique
                    throw new DuplicateIPAddressException(String.Format("Duplicate detected for IP: {0}", details.IP));
            }
            else
                throw new Exception($"IP details are missing for IP: {details.IP}");
        }
    }
}
