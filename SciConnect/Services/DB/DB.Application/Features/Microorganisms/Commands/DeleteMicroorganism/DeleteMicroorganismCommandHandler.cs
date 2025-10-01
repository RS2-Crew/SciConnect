using System;
using System.Threading;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Microorganisms.Commands.DeleteMicroorganism
{
    public class DeleteMicroorganismCommandHandler : IRequestHandler<DeleteMicroorganismCommand, Unit>
    {
        private readonly IMicroorganismRepository _repository;
        private readonly ILogger<DeleteMicroorganismCommandHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public DeleteMicroorganismCommandHandler(
            IMicroorganismRepository repository,
            ILogger<DeleteMicroorganismCommandHandler> logger,
            IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Unit> Handle(DeleteMicroorganismCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Microorganism name must be provided.");

            var microorganism = await _repository.GetByNameAsync(request.Name.Trim());
            if (microorganism == null)
                throw new ArgumentException($"Microorganism with name {request.Name} not found.");

            await _repository.DeleteAsync(microorganism);

            _logger.LogInformation($"Microorganism with name {request.Name} successfully deleted.");

            await _publishEndpoint.Publish(new AutocompleteEntityChanged
            {
                Type = EntityType.Microorganism,
                Kind = ChangeKind.Deleted,
                Id = microorganism.Id,
                Name = null
            });

            return Unit.Value;
        }
    }
}
