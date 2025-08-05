using System.Collections.Generic;
using System.Threading.Tasks;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Persistance
{
    public interface IEmployeeRepository : IAsyncRepository<Employee>
    {
        Task<IReadOnlyCollection<Employee>> GetByFirstAndLastNameAsync(string firstName, string lastName);
        Task<Employee?> GetSingleByNameAsync(string firstName, string lastName);
    }
}
