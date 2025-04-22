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
    public class KeywordRepository : RepositoryBase<Keyword>, IKeywordRepository
    {
        public KeywordRepository(SqlServerContext dbContext) : base(dbContext)
        {
        }

        public async Task<Keyword?> GetByNameAsync(string name)
        {
            return await _dbContext.Keywords
                .FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());
        }

        public async Task<IReadOnlyCollection<Keyword>> GetKeywordByName(string name)
        {
            var keywords = await _dbContext.Keywords
                .Where(o => o.Name == name)
                .ToListAsync();

            return keywords.AsReadOnly();
        }
    }
}
