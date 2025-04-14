using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Persistance
{
    public interface IInstrumentRepository : IAsyncRepository<Instrument>
    {
        Task<IReadOnlyCollection<Instrument>> GetInstrumentByName(string name);
        Task<Instrument?> GetByNameAsync(string name);
    }
}
