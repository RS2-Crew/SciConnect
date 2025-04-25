using System.Collections.Generic;
using DB.Application.Features.Employees.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Employees.Queries.GetAllEmployees
{
    public class GetAllEmployeesQuery : IRequest<IReadOnlyList<EmployeeViewModel>>
    {
    }
}
