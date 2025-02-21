using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Instruments.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Instruments.Queries.GetAllInstruments
{
    public class GetAllInstrumentsQueryHandler : IRequestHandler<GetAllInstrumentsQuery, IReadOnlyList<InstrumentViewModel>>
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentViewModelFactory _viewModelFactory;

        public GetAllInstrumentsQueryHandler(IInstrumentRepository instrumentRepository, IInstrumentViewModelFactory viewModelFactory)
        {
            _instrumentRepository = instrumentRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<IReadOnlyList<InstrumentViewModel>> Handle(GetAllInstrumentsQuery request, CancellationToken cancellationToken)
        {
            var instruments = await _instrumentRepository.GetAllAsync();
            return instruments.Select(_viewModelFactory.CreateViewModel).ToList();
        }
    }
}
