using API.CustomExceptions;
using API.Data;
using API.DataMappers;
using API.Entities;
using API.Repositories;
using API.Services;
using Common.Models;
using IPLibrary;
using IPLibrary.CustomExceptions;
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
        private readonly IIPManagerRepository repository;
        public IPManagerController(IMemoryCache memoryCache, DataContext context, IConfiguration configuration) // Dependency Injection
        {
            this.memoryCache = memoryCache;
            this.context = context;
            this.configuration = configuration;
            repository = new IPManagerRepository(context);
        }

        [Route("/Details/{ip}")]
        [HttpGet]
        public async Task<ActionResult<IPDetails>> Details(string ip)
        {
            try
            {
                // Attempt to retrieve from cache
                if (!memoryCache.TryGetValue(ip, out IPDetails details))
                {
                    // Attempt to retrieve from database
                    IPManagerService managerService = new IPManagerService(repository);
                    details = await managerService.GetByIPAsync(ip);
                    if (details == null)
                    {
                        // Get from IPLibrary
                        IPInfoProviderService providerService = new IPInfoProviderService(configuration);
                        details = providerService.GetDetails(ip);

                        // Save in our Database
                        details = await managerService.SaveAsync(details);
                    }

                    // Save in cache for one minute
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));
                    memoryCache.Set(ip, details, cacheEntryOptions);
                }

                return details;
            }
            catch (IPServiceNotAvailableException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Error getting IP details");
            }
        }

        // Get from Database
        [Route("/GetDetailsFromDB")]
        [HttpGet]
        public async Task<ActionResult<List<IPDetails>>> GetDetailsFromDB()
        {
            try
            {
                IPManagerService managerService = new IPManagerService(repository);
                return await managerService.ListAsync();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                "Error whilst getting IP details from database");
            }
        }

        // Get from Database by IP
        [Route("/GetDetailsFromDB/{ip}")]
        [HttpGet]
        public async Task<ActionResult<IPDetails>> GetDetailsFromDB(string ip)
        {
            try
            {
                IPManagerService managerService = new IPManagerService(repository);
                return await managerService.GetByIPAsync(ip);
            } catch(DuplicateIPAddressException exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            } catch(Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                $"Error whilst getting IP details from database. {exception.Message}");
            }
        }

        //[Route("/BatchUpdate")]
        //[HttpPost]
        //public async Task<ActionResult<int>> BatchUpdate(IPDetailsEntity[])
        //{

        //}
    }
}
