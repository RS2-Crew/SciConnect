using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Instruments.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Instruments.Queries.GetListOfInstruments
{
    public class GetListOfInstrumentsQueryHandler : IRequestHandler<GetListOfInstrumentsQuery, IReadOnlyCollection<InstrumentViewModel>>
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentViewModelFactory _viewModelFactory;

        public GetListOfInstrumentsQueryHandler(IInstrumentRepository instrumentRepository, IInstrumentViewModelFactory viewModelFactory)
        {
            _instrumentRepository = instrumentRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<IReadOnlyCollection<InstrumentViewModel>> Handle(GetListOfInstrumentsQuery request, CancellationToken cancellationToken)
        {
            var instruments = await _instrumentRepository.GetInstrumentByName(request.Name);

            return instruments.Select(i => _viewModelFactory.CreateViewModel(i)).ToList().AsReadOnly();
        }
    }
}
