using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using DB.Domain.Entities;
using DB.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace DB.Infrastructure.Repositories
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(SqlServerContext dbContext) : base(dbContext)
        {
        }

        public async Task<Employee?> GetSingleByNameAsync(string firstName, string lastName)
        {
            return await _dbContext.Employees
                .Include(e => e.Institution)
                .Include(e => e.Keywords)
                .FirstOrDefaultAsync(e =>
                    e.FirstName.ToLower() == firstName.ToLower() &&
                    e.LastName.ToLower() == lastName.ToLower());
        }

        public async Task<IReadOnlyCollection<Employee>> GetByFirstAndLastNameAsync(string firstName, string lastName)
        {
            var employees = await _dbContext.Employees
                .Include(e => e.Institution)
                .Include(e => e.Keywords)
                .Where(e =>
                    e.FirstName.ToLower() == firstName.ToLower() &&
                    e.LastName.ToLower() == lastName.ToLower())
                .ToListAsync();

            return employees.AsReadOnly();
        }
    }
}
