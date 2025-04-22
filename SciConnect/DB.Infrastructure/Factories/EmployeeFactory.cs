using DB.Application.Contracts.Factories;
using DB.Application.Features.Employees.Commands.CreateEmployee;
using DB.Domain.Entities;

namespace DB.Infrastructure.Factories
{
    public class EmployeeFactory : IEmployeeFactory
    {
        public Employee Create(CreateEmployeeCommand command, Institution institution)
        {
            if (string.IsNullOrWhiteSpace(command.FirstName) || string.IsNullOrWhiteSpace(command.LastName))
            {
                throw new ArgumentException("First name and last name must be provided.");
            }

            if (institution == null)
            {
                throw new ArgumentNullException(nameof(institution), "Institution must be provided.");
            }

            return new Employee(command.FirstName, command.LastName, institution);
        }
    }
}
