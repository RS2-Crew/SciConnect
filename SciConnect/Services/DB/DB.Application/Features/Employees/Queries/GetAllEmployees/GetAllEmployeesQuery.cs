using DB.Application.Contracts.Persistance;
using DB.Application.Features.Employees.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Employees.Queries.GetAllEmployees
{
    public class GetAllEmployeesQuery : IRequest<PagedResult<EmployeeViewModel>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
