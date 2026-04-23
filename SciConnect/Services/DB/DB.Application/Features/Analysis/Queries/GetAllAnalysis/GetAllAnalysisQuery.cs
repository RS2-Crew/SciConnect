using DB.Application.Contracts.Persistance;
using DB.Application.Features.Analysis.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Analyses.Queries.GetAllAnalyses
{
    public class GetAllAnalysesQuery : IRequest<PagedResult<AnalysisViewModel>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
