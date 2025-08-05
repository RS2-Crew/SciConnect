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
    public class InstitutionRepository : RepositoryBase<Institution>, IInstitutionRepository
    {
        //public Task<IEnumerable<Institution>> GetInstitutionByName(string name)
        //{
        //    throw new NotImplementedException();
        //}

        public InstitutionRepository(SqlServerContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyCollection<Institution>> GetInstitutionByName(string name)
        {
            var institutions = await _dbContext.Institutions
                .Where(o => o.Name == name)
                .ToListAsync();

            return institutions.AsReadOnly();
        }

        public async Task<Institution?> GetByNameAsync(string name)
        {
            return await _dbContext.Institutions
                                   .FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());
        }
    }
}
