using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using DB.Domain.Entities;
using DB.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace DB.Infrastructure.Repositories
{
    public class InstrumentRepository : RepositoryBase<Instrument>, IInstrumentRepository
    {
        public InstrumentRepository(SqlServerContext dbContext) : base(dbContext)
        {

        }

        public async Task<IReadOnlyCollection<Instrument>> GetInstrumentByName(string name)
        {
            var instruments = await _dbContext.Instruments
                .Where(o => o.Name == name)
                .ToListAsync();

            return instruments.AsReadOnly();
        }
    }
}
