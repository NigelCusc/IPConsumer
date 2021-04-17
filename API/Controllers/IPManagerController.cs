using Common.Models;
using IPLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("IPManager")]
    public class IPManagerController : Controller
    {
        private IMemoryCache memoryCache;
        public IPManagerController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        [Route("IPManager/Details")]
        [HttpGet]
        public IPDetails Details(string ip)
        {
            // Request for IP details
            IPDetails details;
            // Attempt to retrieve from cache
            bool AlreadyExists = memoryCache.TryGetValue(ip, out details);
            if(!AlreadyExists)
            {
                // Attempt to retrieve from database
                // TODO

                // Get from IPLibrary
                IPInfoProviderService service = new IPInfoProviderService();
                details = service.GetDetails(ip);

                // Save in cache for one minute
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));
                memoryCache.Set(ip, details, cacheEntryOptions);
            }

            return details;
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
