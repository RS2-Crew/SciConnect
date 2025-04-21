using DB.Application.Contracts.Factories;
using DB.Application.Features.Employees.Queries.ViewModels;
using DB.Domain.Entities;

namespace DB.Infrastructure.Factories
{
    public class EmployeeViewModelFactory : IEmployeeViewModelFactory
    {
        public EmployeeViewModel CreateViewModel(Employee employee)
        {
            var viewModel = new EmployeeViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName
            };

            return viewModel;
        }
    }
}
