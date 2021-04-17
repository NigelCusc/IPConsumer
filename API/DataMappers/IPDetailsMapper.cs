using API.Entities;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataMappers
{
    public static class IPDetailsMapper
    {
        public static IPDetails ToModel(IPDetailsEntity entity)
        {
            return new IPDetails()
            {
                IP = entity.IP,
                City = entity.City,
                Continent = entity.Continent,
                Country = entity.Country,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude
            };
        }
        public static IPDetailsEntity ToEntity(IPDetails model)
        {
            return new IPDetailsEntity()
            {
                IP = model.IP,
                City = model.City,
                Continent = model.Continent,
                Country = model.Country,
                Latitude = model.Latitude,
                Longitude = model.Longitude
            };
        }
    }
}
