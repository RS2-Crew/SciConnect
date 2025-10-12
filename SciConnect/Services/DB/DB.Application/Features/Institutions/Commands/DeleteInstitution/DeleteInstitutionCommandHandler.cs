using System;
using System.Threading;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Institutions.Commands.DeleteInstitution
{
    public class DeleteInstitutionCommandHandler : IRequestHandler<DeleteInstitutionCommand, Unit>
    {
        private readonly IInstitutionRepository _repository;
        private readonly ILogger<DeleteInstitutionCommandHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public DeleteInstitutionCommandHandler(
            IInstitutionRepository repository,
            ILogger<DeleteInstitutionCommandHandler> logger,
            IPublishEndpoint publishEndpoint)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<Unit> Handle(DeleteInstitutionCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException("Institution name must be provided.");

            var institution = await _repository.GetByNameAsync(request.Name);
            if (institution == null)
                throw new ArgumentException($"Institution with name {request.Name} not found.");

            await _repository.DeleteAsync(institution);

            _logger.LogInformation("Institution with name {Name} successfully deleted.", request.Name);

            await _publishEndpoint.Publish(new AutocompleteEntityChanged
            {
                Type = EntityType.Institution,
                Kind = ChangeKind.Deleted,
                Id = institution.Id,
                Name = null
            });

            return Unit.Value;
        }
    }
}
