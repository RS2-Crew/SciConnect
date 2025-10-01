using System;
using System.Threading;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Institutions.Commands.CreateInstitution;
using EventBus.Messages.Entities;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Instruments.Commands.CreateInstrument
{
    class CreateInstrumentCommandHandler : IRequestHandler<CreateInstrumentCommand, int>
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentFactory _instrumentFactory;
        private readonly ILogger<CreateInstrumentCommandHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateInstrumentCommandHandler(IInstrumentRepository instrumentRepository, IInstrumentFactory instrumentFactory, ILogger<CreateInstrumentCommandHandler> logger, IPublishEndpoint publishEndpoint)
        {
            _instrumentRepository = instrumentRepository ?? throw new ArgumentNullException(nameof(instrumentRepository));
            _instrumentFactory = instrumentFactory ?? throw new ArgumentNullException(nameof(instrumentFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<int> Handle(CreateInstrumentCommand request, CancellationToken cancellationToken)
        {
            var instrument = _instrumentFactory.Create(request);

            await _instrumentRepository.AddAsync(instrument);

            var evt = new SimpleEntityCreatedEvent
            {
                EntityType = "Instrument",
                Name = instrument.Name
            };

            await _publishEndpoint.Publish(evt);

            await _publishEndpoint.Publish(new AutocompleteEntityChanged
            {
                Type = EntityType.Instrument,
                Kind = ChangeKind.Created,
                Id = instrument.Id,
                Name = instrument.Name
            });

            return instrument.Id;
        }
    }
}
