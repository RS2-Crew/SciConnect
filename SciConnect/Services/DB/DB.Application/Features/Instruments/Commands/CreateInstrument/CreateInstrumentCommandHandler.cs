using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Institutions.Commands.CreateInstitution;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Instruments.Commands.CreateInstrument
{
    class CreateInstrumentCommandHandler : IRequestHandler<CreateInstrumentCommand, int>
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentFactory _instrumentFactory;
        private readonly ILogger<CreateInstrumentCommandHandler> _logger;

        public CreateInstrumentCommandHandler(IInstrumentRepository instrumentRepository, IInstrumentFactory instrumentFactory, ILogger<CreateInstrumentCommandHandler> logger)
        {
            _instrumentRepository = instrumentRepository;
            _instrumentFactory = instrumentFactory;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(CreateInstrumentCommand request, CancellationToken cancellationToken)
        {
            var instrument = _instrumentFactory.Create(request);

            await _instrumentRepository.AddAsync(instrument);

            return instrument.Id;
        }
    }
}
