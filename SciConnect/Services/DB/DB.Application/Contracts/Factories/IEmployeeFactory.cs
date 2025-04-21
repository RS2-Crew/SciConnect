using DB.Application.Features.Employees.Commands.CreateEmployee;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Factories
{
    public interface IEmployeeFactory
    {
        Employee Create(CreateEmployeeCommand command, Institution institution);
    }
}
