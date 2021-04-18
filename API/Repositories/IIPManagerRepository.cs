using API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface IIPManagerRepository
    {
        Task<List<IPDetailsEntity>> ListAsync();
        Task<IPDetailsEntity> FindByIdAsync(int id);
        Task<List<IPDetailsEntity>> FindByIPAsync(string ip);
        Task<IPDetailsEntity> AddAsync(IPDetailsEntity detailsEntity);
        IPDetailsEntity UpdateAsync(IPDetailsEntity detailsEntity);
    }
}
