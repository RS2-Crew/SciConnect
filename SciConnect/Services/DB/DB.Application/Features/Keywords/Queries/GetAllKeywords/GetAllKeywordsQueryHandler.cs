using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Keywords.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Keywords.Queries.GetAllKeywords
{
    public class GetAllKeywordsQueryHandler : IRequestHandler<GetAllKeywordsQuery, IReadOnlyList<KeywordViewModel>>
    {
        private readonly IKeywordRepository _keywordRepository;
        private readonly IKeywordViewModelFactory _viewModelFactory;

        public GetAllKeywordsQueryHandler(IKeywordRepository keywordRepository, IKeywordViewModelFactory viewModelFactory)
        {
            _keywordRepository = keywordRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<IReadOnlyList<KeywordViewModel>> Handle(GetAllKeywordsQuery request, CancellationToken cancellationToken)
        {
            var keywords = await _keywordRepository.GetAllAsync();
            return keywords.Select(_viewModelFactory.CreateViewModel).ToList();
        }
    }
}
