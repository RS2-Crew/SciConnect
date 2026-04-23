using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Keywords.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Keywords.Queries.GetAllKeywords
{
    public class GetAllKeywordsQueryHandler : IRequestHandler<GetAllKeywordsQuery, PagedResult<KeywordViewModel>>
    {
        private readonly IKeywordRepository _keywordRepository;
        private readonly IKeywordViewModelFactory _viewModelFactory;

        public GetAllKeywordsQueryHandler(IKeywordRepository keywordRepository, IKeywordViewModelFactory viewModelFactory)
        {
            _keywordRepository = keywordRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<PagedResult<KeywordViewModel>> Handle(GetAllKeywordsQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _keywordRepository.GetPagedAsync(request.PageNumber, request.PageSize);
            var viewModels = pagedResult.Items.Select(_viewModelFactory.CreateViewModel).ToList().AsReadOnly();
            return new PagedResult<KeywordViewModel>(viewModels, pagedResult.TotalCount, pagedResult.PageNumber, pagedResult.PageSize);
        }
    }
}
