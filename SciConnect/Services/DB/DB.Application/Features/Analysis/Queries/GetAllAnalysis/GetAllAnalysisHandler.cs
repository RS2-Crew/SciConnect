using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Analysis.Queries.ViewModels;

using MediatR;

namespace DB.Application.Features.Analyses.Queries.GetAllAnalyses
{
    public class GetAllAnalysesQueryHandler : IRequestHandler<GetAllAnalysesQuery, IReadOnlyList<AnalysisViewModel>>
    {
        private readonly IAnalysisRepository _analysisRepository;
        private readonly IAnalysisViewModelFactory _viewModelFactory;

        public GetAllAnalysesQueryHandler(IAnalysisRepository analysisRepository, IAnalysisViewModelFactory viewModelFactory)
        {
            _analysisRepository = analysisRepository ?? throw new ArgumentNullException(nameof(analysisRepository));
            _viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));
        }

        public async Task<IReadOnlyList<AnalysisViewModel>> Handle(GetAllAnalysesQuery request, CancellationToken cancellationToken)
        {
            var analyses = await _analysisRepository.GetAllAsync();
            return analyses
                .Select(_viewModelFactory.CreateViewModel)
                .ToList()
                .AsReadOnly();
        }
    }
}