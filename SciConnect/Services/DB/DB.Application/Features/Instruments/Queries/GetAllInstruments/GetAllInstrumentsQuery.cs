using DB.Application.Contracts.Persistance;
using DB.Application.Features.Instruments.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Instruments.Queries.GetAllInstruments
{
    public class GetAllInstrumentsQuery : IRequest<PagedResult<InstrumentViewModel>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
