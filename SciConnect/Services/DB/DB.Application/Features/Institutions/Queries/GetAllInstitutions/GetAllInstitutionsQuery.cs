using DB.Application.Contracts.Persistance;
using DB.Application.Features.Institutions.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Institutions.Queries.GetAllInstitutions
{
    public class GetAllInstitutionsQuery : IRequest<PagedResult<InstitutionViewModel>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
