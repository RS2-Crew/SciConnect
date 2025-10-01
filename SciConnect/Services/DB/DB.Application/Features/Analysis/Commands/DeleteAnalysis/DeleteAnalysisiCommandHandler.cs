using System;
using System.Threading;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Analyses.Commands.DeleteAnalysis
{
    public class DeleteAnalysisCommandHandler : IRequestHandler<DeleteAnalysisCommand, Unit>
    {
        private readonly IAnalysisRepository _repository;
        private readonly ILogger<DeleteAnalysisCommandHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public DeleteAnalysisCommandHandler(
            IAnalysisRepository repository,
            ILogger<DeleteAnalysisCommandHandler> logger,
            IPublishEndpoint publishEndpoint)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<Unit> Handle(DeleteAnalysisCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Analysis name must be provided.");

            var analysis = await _repository.GetByNameAsync(request.Name.Trim());
            if (analysis == null)
                throw new ArgumentException($"Analysis with name '{request.Name}' not found.");

            await _repository.DeleteAsync(analysis);

            _logger.LogInformation("Analysis with name '{AnalysisName}' successfully deleted.", request.Name);

            await _publishEndpoint.Publish(new AutocompleteEntityChanged
            {
                Type = EntityType.Analysis,
                Kind = ChangeKind.Deleted,
                Id = analysis.Id,
                Name = null
            }, cancellationToken);

            return Unit.Value;
        }
    }
}
