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

namespace DB.Application.Features.Institutions.Queries.GetInstitutionWithAnalyses
{
    public class GetAnalysesByInstitutionQueryHandler : IRequestHandler<GetAnalysesByInstitutionQuery, InstitutionViewModel>
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IAnalysisViewModelFactory _analysisVMFactory;

        public GetAnalysesByInstitutionQueryHandler(IInstitutionRepository institutionRepository, IAnalysisViewModelFactory analysisVMFactory)
        {
            _institutionRepository = institutionRepository;
            _analysisVMFactory = analysisVMFactory;
        }

        public async Task<InstitutionViewModel> Handle(GetAnalysesByInstitutionQuery request, CancellationToken cancellationToken)
        {
            var institution = await _institutionRepository
               .GetQueryable()
               .Include(i => i.Analyses)
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
                Analyses = institution.Analyses
                    .Select(i => _analysisVMFactory.CreateViewModel(i))
                    .ToList()
            };

            return viewModel;
        }
    }
}
