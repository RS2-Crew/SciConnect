using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Instruments.Commands.DeleteInstrument
{
    public class DeleteInstrumentCommandHandler : IRequestHandler<DeleteInstrumentCommand, Unit>
    {
        private readonly IInstrumentRepository _repository;
        private readonly ILogger<DeleteInstrumentCommandHandler> _logger;

        public DeleteInstrumentCommandHandler(IInstrumentRepository repository, ILogger<DeleteInstrumentCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteInstrumentCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                _logger.LogError("Instrument name must be provided.");
                throw new ArgumentException("Instrument name must be provided.");
            }

            var instrument = await _repository.GetByNameAsync(request.Name.Trim());

            if (instrument == null)
            {
                _logger.LogError($"Instrument with name {request.Name} not found.");
                throw new ArgumentException($"Instrument with name {request.Name} not found.");
            }

            await _repository.DeleteAsync(instrument);

            _logger.LogInformation($"Instrument with name {request.Name} successfully deleted.");

            return Unit.Value;
        }
    }
}
