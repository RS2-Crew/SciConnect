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
    public class AnalysisRepository : RepositoryBase<Analysis>, IAnalysisRepository
    {
        public AnalysisRepository(SqlServerContext dbContext) : base(dbContext)
        {
        }

        public async Task<Analysis?> GetByNameAsync(string name)
        {
            return await _dbContext.Analyses.FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());
        }

        public async Task<IReadOnlyCollection<Analysis>> GetAnalysisByName(string name)
        {
            var analysis = await _dbContext.Analyses
                .Where(a => a.Name == name)
                .ToListAsync();

            return analysis.AsReadOnly();
        }

        public async Task<IReadOnlyList<Analysis>> GetAllAsync()
        {
            return await _dbContext.Analyses
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
