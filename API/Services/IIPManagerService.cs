using Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    interface IIPManagerService
    {
        Task<List<IPDetails>> ListAsync();
        Task<IPDetails> GetByIPAsync(string ip);
        Task<IPDetails> SaveAsync(IPDetails details);
        Task<IPDetails> UpdateAsync(IPDetails details);
    }
}
