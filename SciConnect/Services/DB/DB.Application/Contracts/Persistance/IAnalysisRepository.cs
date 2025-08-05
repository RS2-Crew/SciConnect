using DB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Application.Contracts.Persistance
{
    public interface IAnalysisRepository : IAsyncRepository<Analysis>
    {
        Task<Analysis?> GetByNameAsync(string name);

        //Task<IEnumerable<Institution>> GetInstitutionByName(string name);
        Task<IReadOnlyCollection<Analysis>> GetAnalysisByName(string name);
    }
}
