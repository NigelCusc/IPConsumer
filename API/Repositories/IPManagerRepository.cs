using API.Data;
using API.DataMappers;
using API.Entities;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class IPManagerRepository : IIPManagerRepository
    {
        private readonly DataContext context;
        public IPManagerRepository(DataContext context) // Dependency Injection
        {
            this.context = context;
        }

        // Get IP details by ID
        public async Task<IPDetailsEntity> FindByIdAsync(int id)
        {
            return await context.IPDetails.FindAsync(id);
        }

        // Get IP details by IP
        public async Task<List<IPDetailsEntity>> FindByIPAsync(string ip)
        {
            return await context.IPDetails.Where(x => x.IP == ip).ToListAsync();
        }

        // Return list of all IP details in database
        public async Task<List<IPDetailsEntity>> ListAsync()
        {
            return await context.IPDetails.ToListAsync();
        }

        // Add new IP details in database
        public async Task<IPDetailsEntity> AddAsync(IPDetailsEntity detailsEntity)
        {
            await context.AddAsync(detailsEntity);

            context.SaveChanges();

            return detailsEntity;
        }

        // Update an IP details record in the database. Using IP as identifier
        public IPDetailsEntity UpdateAsync(IPDetailsEntity detailsEntity)
        {
            context.IPDetails.Update(detailsEntity);

            context.SaveChanges();

            return detailsEntity;
        }
    }
}
