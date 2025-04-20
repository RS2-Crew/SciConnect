using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Persistance
{
    public interface IKeywordRepository : IAsyncRepository<Keyword>
    {
        Task<IReadOnlyCollection<Keyword>> GetKeywordByName(string name);
        Task<Keyword?> GetByNameAsync(string name);
    }
}
