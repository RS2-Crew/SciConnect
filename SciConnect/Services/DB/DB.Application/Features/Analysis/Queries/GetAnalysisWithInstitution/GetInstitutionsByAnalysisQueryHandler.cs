using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Analysis.Queries.GetAnalysisWithMicroorganism;
using DB.Application.Features.Analysis.Queries.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DB.Application.Features.Analysis.Queries.GetAnalysisWithInstitution
{
    public class GetInstitutionsByAnalysisQueryHandler : IRequestHandler<GetInstitutionsByAnalysisQuery, AnalysisViewModel>
    {

        private readonly IAnalysisRepository _analysisRepository;
        private readonly IInstitutionViewModelFactory _institutionVMFactory;

        public GetInstitutionsByAnalysisQueryHandler(IAnalysisRepository analysisRepository, IInstitutionViewModelFactory institutionVMFactory)
        {
            _analysisRepository = analysisRepository;
            _institutionVMFactory = institutionVMFactory;
        }

        public async Task<AnalysisViewModel> Handle(GetInstitutionsByAnalysisQuery request, CancellationToken cancellationToken)
        {
            var analysis = await _analysisRepository
                .GetQueryable()
                .Include(a => a.Institutions)
                .FirstOrDefaultAsync(a => a.Name.ToLower() == request.AnalysisName.ToLower(), cancellationToken);

            if (analysis == null)
                return null;

            var viewModel = new AnalysisViewModel
            {
                Id = analysis.Id,
                Name = analysis.Name,
                Institutions = analysis.Institutions
                    .Select(m => _institutionVMFactory.CreateViewModel(m))
                    .ToList()
            };

            return viewModel;
        }
    }
}
