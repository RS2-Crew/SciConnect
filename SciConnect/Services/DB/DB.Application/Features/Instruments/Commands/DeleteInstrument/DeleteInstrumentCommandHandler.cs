using System;
using System.Threading;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Instruments.Commands.DeleteInstrument
{
    public class DeleteInstrumentCommandHandler : IRequestHandler<DeleteInstrumentCommand, Unit>
    {
        private readonly IInstrumentRepository _repository;
        private readonly ILogger<DeleteInstrumentCommandHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public DeleteInstrumentCommandHandler(IInstrumentRepository repository, ILogger<DeleteInstrumentCommandHandler> logger, IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Unit> Handle(DeleteInstrumentCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Instrument name must be provided.");

            var instrument = await _repository.GetByNameAsync(request.Name.Trim());
            if (instrument == null)
                throw new ArgumentException($"Instrument with name {request.Name} not found.");

            await _repository.DeleteAsync(instrument);

            _logger.LogInformation("Instrument with name {Name} successfully deleted.", request.Name);

            await _publishEndpoint.Publish(new AutocompleteEntityChanged
            {
                Type = EntityType.Instrument,
                Kind = ChangeKind.Deleted,
                Id = instrument.Id,
                Name = null
            });

            return Unit.Value;
        }
    }
}
