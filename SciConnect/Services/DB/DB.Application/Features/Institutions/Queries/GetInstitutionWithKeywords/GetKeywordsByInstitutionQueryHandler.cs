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

namespace DB.Application.Features.Institutions.Queries.GetInstitutionWithKeywords
{
    public class GetKeywordsByInstitutionQueryHandler : IRequestHandler<GetKeywordsByInstitutionQuery, InstitutionViewModel>
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IKeywordViewModelFactory _keywordVMFactory;

        public GetKeywordsByInstitutionQueryHandler(IInstitutionRepository institutionRepository, IKeywordViewModelFactory keywordVMFactory)
        {
            _institutionRepository = institutionRepository;
            _keywordVMFactory = keywordVMFactory;
        }

        public async Task<InstitutionViewModel> Handle(GetKeywordsByInstitutionQuery request, CancellationToken cancellationToken)
        {
            var institution = await _institutionRepository
               .GetQueryable()
               .Include(i => i.Keywords)
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
                Keywords = institution.Keywords
                    .Select(i => _keywordVMFactory.CreateViewModel(i))
                    .ToList()
            };

            return viewModel;
        }
    }
}
