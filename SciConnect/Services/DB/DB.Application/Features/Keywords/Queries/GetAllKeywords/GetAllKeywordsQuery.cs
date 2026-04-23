using DB.Application.Contracts.Persistance;
using DB.Application.Features.Keywords.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Keywords.Queries.GetAllKeywords
{
    public class GetAllKeywordsQuery : IRequest<PagedResult<KeywordViewModel>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
