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
    public class MicroorganismRepository : RepositoryBase<Microorganism>, IMicroorganismRepository
    {
        public MicroorganismRepository(SqlServerContext dbContext) : base(dbContext)
        {
        }
        public async Task<Microorganism?> GetByNameAsync(string name)
        {
            return await _dbContext.Microorganisms
                                   .FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());
        }

        public async Task<IReadOnlyCollection<Microorganism>> GetMicroorganismByName(string name)
        {
            var microorganisms = await _dbContext.Microorganisms
                .Where(o => o.Name == name)
                .ToListAsync();

            return microorganisms.AsReadOnly();
        }

    }
}
