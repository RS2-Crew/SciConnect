using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Employees.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Employees.Queries.GetAllEmployees
{
    public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, PagedResult<EmployeeViewModel>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeViewModelFactory _viewModelFactory;

        public GetAllEmployeesQueryHandler(IEmployeeRepository employeeRepository, IEmployeeViewModelFactory viewModelFactory)
        {
            _employeeRepository = employeeRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<PagedResult<EmployeeViewModel>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _employeeRepository.GetPagedAsync(request.PageNumber, request.PageSize);
            var viewModels = pagedResult.Items.Select(_viewModelFactory.CreateViewModel).ToList().AsReadOnly();
            return new PagedResult<EmployeeViewModel>(viewModels, pagedResult.TotalCount, pagedResult.PageNumber, pagedResult.PageSize);
        }
    }
}
