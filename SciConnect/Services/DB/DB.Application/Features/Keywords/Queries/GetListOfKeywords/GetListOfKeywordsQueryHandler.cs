using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Microorganisms.Queries.ViewModels;
using DB.Application.Features.Microorganisms.Queries;
using MediatR;
using DB.Application.Features.Keywords.Queries.ViewModels;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;

namespace DB.Application.Features.Keywords.Queries.GetListOfKeywords
{
    public class GetListOfKeywordsQueryHandler : IRequestHandler<GetListOfKeywordsQuery, IReadOnlyCollection<KeywordViewModel>>
    {
        private readonly IKeywordRepository _keywordRepository;
        private readonly IKeywordViewModelFactory _viewModelFactory;

        public GetListOfKeywordsQueryHandler(IKeywordRepository keywordRepository, IKeywordViewModelFactory viewModelFactory)
        {
            _keywordRepository = keywordRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<IReadOnlyCollection<KeywordViewModel>> Handle(GetListOfKeywordsQuery request, CancellationToken cancellationToken)
        {
            var keywords = await _keywordRepository.GetKeywordByName(request.Name);

            return keywords.Select(i => _viewModelFactory.CreateViewModel(i)).ToList().AsReadOnly();
        }

    }
}
