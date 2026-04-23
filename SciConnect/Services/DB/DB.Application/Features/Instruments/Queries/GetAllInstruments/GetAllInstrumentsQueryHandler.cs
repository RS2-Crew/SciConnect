using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Instruments.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Instruments.Queries.GetAllInstruments
{
    public class GetAllInstrumentsQueryHandler : IRequestHandler<GetAllInstrumentsQuery, PagedResult<InstrumentViewModel>>
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentViewModelFactory _viewModelFactory;

        public GetAllInstrumentsQueryHandler(IInstrumentRepository instrumentRepository, IInstrumentViewModelFactory viewModelFactory)
        {
            _instrumentRepository = instrumentRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<PagedResult<InstrumentViewModel>> Handle(GetAllInstrumentsQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _instrumentRepository.GetPagedAsync(request.PageNumber, request.PageSize);
            var viewModels = pagedResult.Items.Select(_viewModelFactory.CreateViewModel).ToList().AsReadOnly();
            return new PagedResult<InstrumentViewModel>(viewModels, pagedResult.TotalCount, pagedResult.PageNumber, pagedResult.PageSize);
        }
    }
}
