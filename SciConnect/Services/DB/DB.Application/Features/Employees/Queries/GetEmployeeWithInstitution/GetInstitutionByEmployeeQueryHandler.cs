using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;

using DB.Application.Features.Employees.Queries.ViewModels;
using DB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DB.Application.Features.Employees.Queries.GetEmployeeWithInstitution
{
    public class GetInstituionByEmployeeQueryHandler : IRequestHandler<GetInstitutionByEmployeeQuery, EmployeeViewModel>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IInstitutionViewModelFactory _instituionVMFactory;

        public GetInstituionByEmployeeQueryHandler(IEmployeeRepository employeeRepository, IInstitutionViewModelFactory institutionVMFactory)
        {
            _employeeRepository = employeeRepository;
            _instituionVMFactory = institutionVMFactory;
        }

        public async Task<EmployeeViewModel> Handle(GetInstitutionByEmployeeQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository
               .GetQueryable()
               .Include(i => i.Institution)
               .FirstOrDefaultAsync(i => i.FirstName.ToLower() == request.EmployeeFirstName.ToLower() &&  
               i.LastName.ToLower() == request.EmployeeLastName.ToLower(), cancellationToken);

            if (employee == null)
                return null;

            var viewModel = new EmployeeViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName= employee.LastName,
                Institution =  _instituionVMFactory.CreateViewModel(employee.Institution)

            };

            return viewModel;
        }

    }
}
