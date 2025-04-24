using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Analysis.Queries.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DB.Application.Features.Analysis.Queries.GetAnalysisWithMicroorganism
{
    public class GetMicroorganismsByAnalysisQueryHandler : IRequestHandler<GetMicroorganismsByAnalysisQuery, AnalysisViewModel>
    {
        private readonly IAnalysisRepository _analysisRepository;
        private readonly IMicroorganismViewModelFactory _microorganismVMFactory;

        public GetMicroorganismsByAnalysisQueryHandler(IAnalysisRepository analysisRepository, IMicroorganismViewModelFactory microorganismVMFactory)
        {
            _analysisRepository = analysisRepository;
            _microorganismVMFactory = microorganismVMFactory;
        }

        public async Task<AnalysisViewModel> Handle(GetMicroorganismsByAnalysisQuery request, CancellationToken cancellationToken)
        {
            var analysis = await _analysisRepository
                .GetQueryable()
                .Include(a => a.Microorganisms)
                .FirstOrDefaultAsync(a => a.Name.ToLower() == request.AnalysisName.ToLower(), cancellationToken);

            if (analysis == null)
                return null;

            var viewModel = new AnalysisViewModel
            {
                Id = analysis.Id,
                Name = analysis.Name,
                Microorganisms = analysis.Microorganisms
                    .Select(m => _microorganismVMFactory.CreateViewModel(m))
                    .ToList()
            };

            return viewModel;
        }
    }
}
