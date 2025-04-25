using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Employees.Queries.GetListOfEmployees;
using DB.Application.Features.Employees.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Employees.Queries.GetListOfEmployees
{
    public class GetListOfEmployeesQueryHandler : IRequestHandler<GetListOfEmployeesQuery, IReadOnlyCollection<EmployeeViewModel>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeViewModelFactory _viewModelFactory;

        public GetListOfEmployeesQueryHandler(IEmployeeRepository employeeRepository, IEmployeeViewModelFactory viewModelFactory)
        {
            _employeeRepository = employeeRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<IReadOnlyCollection<EmployeeViewModel>> Handle(GetListOfEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await _employeeRepository.GetByFirstAndLastNameAsync(request.FirstName, request.LastName);
            return employees.Select(_viewModelFactory.CreateViewModel).ToList().AsReadOnly();
        }
    }
}
