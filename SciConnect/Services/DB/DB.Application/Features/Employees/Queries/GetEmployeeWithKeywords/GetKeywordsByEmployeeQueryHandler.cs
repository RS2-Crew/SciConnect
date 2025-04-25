using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Employees.Queries.GetEmployeeWithInstitution;
using DB.Application.Features.Employees.Queries.ViewModels;
using DB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Application.Features.Employees.Queries.GetEmployeeWithKeywords
{
    public class GetKeywordsByEmployeeQueryHandler : IRequestHandler<GetKeywordsByEmployeeQuery, EmployeeViewModel>
    {

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IKeywordViewModelFactory _keywordVMFactory;

        public GetKeywordsByEmployeeQueryHandler(IEmployeeRepository employeeRepository, IKeywordViewModelFactory keywordVMFactory)
        {
            _employeeRepository = employeeRepository;
            _keywordVMFactory = keywordVMFactory;
        }

        public async Task<EmployeeViewModel> Handle(GetKeywordsByEmployeeQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository
               .GetQueryable()
               .Include(i => i.Keywords)
               .FirstOrDefaultAsync(i => i.FirstName.ToLower() == request.EmployeeFirstName.ToLower() &&
               i.LastName.ToLower() == request.EmployeeLastName.ToLower(), cancellationToken);

            if (employee == null)
                return null;

            var viewModel = new EmployeeViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName= employee.LastName,
                Keywords = employee.Keywords
                    .Select(m => _keywordVMFactory.CreateViewModel(m))
                    .ToList()

            };

            return viewModel;
        }

    }
}

