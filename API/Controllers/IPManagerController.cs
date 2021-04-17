using API.Data;
using API.Entities;
using Common.Models;
using IPLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IPManagerController : ControllerBase
    {
        private IMemoryCache memoryCache;
        private readonly DataContext context;
        private readonly IConfiguration configuration;
        public IPManagerController(IMemoryCache memoryCache, DataContext context, IConfiguration configuration) // Dependency Injection
        {
            this.memoryCache = memoryCache;
            this.context = context;
            this.configuration = configuration;
        }

        [Route("/Details/{ip}")]
        [HttpGet]
        public async Task<IPDetails> Details(string ip)
        {
            // Request for IP details
            IPDetails details;
            // Attempt to retrieve from cache
            if(!memoryCache.TryGetValue(ip, out details))
            {
                // Attempt to retrieve from database
                details = await GetDetailsFromDB(ip);
                if (details == null)
                {
                    // Get from IPLibrary
                    IPInfoProviderService service = new IPInfoProviderService(configuration);
                    details = service.GetDetails(ip);

                    // Save in our Database

                }

                // Save in cache for one minute
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));
                memoryCache.Set(ip, details, cacheEntryOptions);
            }

            return details;
        }

        // Get from Database
        [Route("/GetDetailsFromDB")]
        [HttpGet]
        public async Task<List<IPDetails>> GetDetailsFromDB()
        {
            List<IPDetails> iPDetailsList = new List<IPDetails>();
            // From DB
            foreach(IPDetailsEntity entity in  await context.IPDetails.ToListAsync())
            {
                iPDetailsList.Add(new IPDetails()
                {
                    City = entity.City,
                    Continent = entity.Continent,
                    Country = entity.Country,
                    Latitude = entity.Latitude,
                    Longitude = entity.Longitude
                });
            }

            return iPDetailsList;
        }

        // Get from Database by IP
        [Route("/GetDetailsFromDB/{ip}")]
        [HttpGet]
        public async Task<IPDetails> GetDetailsFromDB(string ip)
        {
            List<IPDetailsEntity> iPDetailsEntitiesList = await context.IPDetails.Where(x => x.IP == ip).ToListAsync();
            if (iPDetailsEntitiesList.Any())
            {
                if (iPDetailsEntitiesList.Count == 1)  // Found one
                {
                    var entity = iPDetailsEntitiesList.First();
                    return new IPDetails()
                    {
                        City = entity.City,
                        Continent = entity.Continent,
                        Country = entity.Country,
                        Latitude = entity.Latitude,
                        Longitude = entity.Longitude
                    };
                }
                else    // Found multiple. Irregular behaviour as IP should be unique
                    throw new Exception(String.Format("Duplicate detected for IP: {0}", ip));
            }
            else
                return null;
        }



        //[Route("IPManager/Create")]
        //[HttpPost]
        //public int Create(string ip)
        //{
        //    return 1;
        //}

        //[Route("IPManager/Edit")]
        //[HttpPost]
        //public int Edit(int id, IPDetails details)
        //{
        //    return 1;
        //}

        //[Route("IPManager/Delete")]
        //[HttpPost]
        //public int Delete(int id)
        //{
        //    return 1;
        //}
    }
}
