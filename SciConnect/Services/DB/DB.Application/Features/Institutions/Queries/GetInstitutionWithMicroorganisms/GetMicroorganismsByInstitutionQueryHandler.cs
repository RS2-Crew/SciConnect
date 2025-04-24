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

namespace DB.Application.Features.Institutions.Queries.GetInstitutionWithMicroorganisms
{
    public class GetMicroorganismsByInstitutionQueryHandler : IRequestHandler<GetMicroorganismsByInstitutionQuery, InstitutionViewModel>
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IMicroorganismViewModelFactory _microorganismVMFactory;

        public GetMicroorganismsByInstitutionQueryHandler(IInstitutionRepository institutionRepository, IMicroorganismViewModelFactory microorganismVMFactory)
        {
            _institutionRepository = institutionRepository;
            _microorganismVMFactory = microorganismVMFactory;
        }

        public async Task<InstitutionViewModel> Handle(GetMicroorganismsByInstitutionQuery request, CancellationToken cancellationToken)
        {
            var institution = await _institutionRepository
               .GetQueryable()
               .Include(i => i.Microorganisms)
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
                Microorganisms = institution.Microorganisms
                    .Select(i => _microorganismVMFactory.CreateViewModel(i))
                    .ToList()
            };

            return viewModel;
        }
    }
}
