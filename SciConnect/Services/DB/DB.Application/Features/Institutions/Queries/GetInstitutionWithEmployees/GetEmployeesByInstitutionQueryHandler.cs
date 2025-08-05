using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Institutions.Queries.GetInstitutionWithInstruments;
using DB.Application.Features.Institutions.Queries.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DB.Application.Features.Institutions.Queries.GetInstitutionWithEmployees
{
    public class GetEmployeesByInstitutionQueryHandler : IRequestHandler<GetEmployeesByInstitutionQuery, InstitutionViewModel>
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IEmployeeViewModelFactory _employeeVMFactory;

        public GetEmployeesByInstitutionQueryHandler(IInstitutionRepository institutionRepository, IEmployeeViewModelFactory employeeVMFactory)
        {
            _institutionRepository = institutionRepository;
            _employeeVMFactory = employeeVMFactory;
        }

        public async Task<InstitutionViewModel> Handle(GetEmployeesByInstitutionQuery request, CancellationToken cancellationToken)
        {
            var institution = await _institutionRepository
               .GetQueryable()
               .Include(i => i.Employees)
               .FirstOrDefaultAsync(i => i.Name.ToLower() == request.InstitutionName.ToLower(), cancellationToken);

            if (institution == null)
                return null;

            var viewModel = new InstitutionViewModel
            {
                Id = institution.Id,
                Name = institution.Name,
                Street = institution.Street,
                StreetNumber = institution.StreetNumber,
                City = institution.City,
                Country = institution.Country,
                Phone = institution.Phone,
                Email = institution.Email,
                Website = institution.Website,
                Employees = institution.Employees
                    .Select(i => _employeeVMFactory.CreateViewModel(i))
                    .ToList()
            };

            return viewModel;
        }
    }
}
