using DB.Application.Contracts.Persistance;
using DB.Application.Features.Microorganisms.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Microorganisms.Queries.GetAllMicroorganisms
{
    public class GetAllMicroorganismsQuery : IRequest<PagedResult<MicroorganismViewModel>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
