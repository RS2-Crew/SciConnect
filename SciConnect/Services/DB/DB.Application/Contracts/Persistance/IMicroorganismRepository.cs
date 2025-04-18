using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Persistance
{
    public interface IMicroorganismRepository : IAsyncRepository<Microorganism>
    {
        Task<IReadOnlyCollection<Microorganism>> GetMicroorganismByName(string name);
        Task<Microorganism?> GetByNameAsync(string name);
    }
}
