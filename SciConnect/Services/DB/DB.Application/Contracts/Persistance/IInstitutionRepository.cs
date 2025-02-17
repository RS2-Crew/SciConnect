using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Persistance
{
    public interface IInstitutionRepository : IAsyncRepository<Institution>
    {

        //Task<IEnumerable<Institution>> GetInstitutionByName(string name);
        Task<IReadOnlyCollection<Institution>> GetInstitutionByName(string name);
    }
}
