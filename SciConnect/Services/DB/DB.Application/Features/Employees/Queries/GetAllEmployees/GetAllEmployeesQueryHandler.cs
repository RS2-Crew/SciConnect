using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Employees.Queries.GetAllEmployees;
using DB.Application.Features.Employees.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Employees.Queries.GetAllEmployees
{
    public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, IReadOnlyList<EmployeeViewModel>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeViewModelFactory _viewModelFactory;

        public GetAllEmployeesQueryHandler(IEmployeeRepository employeeRepository, IEmployeeViewModelFactory viewModelFactory)
        {
            _employeeRepository = employeeRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<IReadOnlyList<EmployeeViewModel>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await _employeeRepository.GetAllAsync();
            return employees.Select(_viewModelFactory.CreateViewModel).ToList();
        }
    }
}
