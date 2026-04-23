using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Analysis.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Analyses.Queries.GetAllAnalyses
{
    public class GetAllAnalysesQueryHandler : IRequestHandler<GetAllAnalysesQuery, PagedResult<AnalysisViewModel>>
    {
        private readonly IAnalysisRepository _analysisRepository;
        private readonly IAnalysisViewModelFactory _viewModelFactory;

        public GetAllAnalysesQueryHandler(IAnalysisRepository analysisRepository, IAnalysisViewModelFactory viewModelFactory)
        {
            _analysisRepository = analysisRepository ?? throw new ArgumentNullException(nameof(analysisRepository));
            _viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));
        }

        public async Task<PagedResult<AnalysisViewModel>> Handle(GetAllAnalysesQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _analysisRepository.GetPagedAsync(request.PageNumber, request.PageSize);
            var viewModels = pagedResult.Items.Select(_viewModelFactory.CreateViewModel).ToList().AsReadOnly();
            return new PagedResult<AnalysisViewModel>(viewModels, pagedResult.TotalCount, pagedResult.PageNumber, pagedResult.PageSize);
        }
    }
}