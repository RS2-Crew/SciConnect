using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Domain.Entities;
using EventBus.Messages.Entities;
using EventBus.Messages.Events;   
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Analyses.Commands.CreateAnalysis
{
    public class CreateAnalysisCommandHandler : IRequestHandler<CreateAnalysisCommand, int>
    {
        private readonly IAnalysisRepository _repository;
        private readonly IAnalysisFactory _factory;
        private readonly ILogger<CreateAnalysisCommandHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateAnalysisCommandHandler(
            IAnalysisRepository repository,
            IAnalysisFactory factory,
            ILogger<CreateAnalysisCommandHandler> logger,
            IPublishEndpoint publishEndpoint)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<int> Handle(CreateAnalysisCommand request, CancellationToken cancellationToken)
        {
            var analysisEntity = _factory.Create(request);
            var newAnalysis = await _repository.AddAsync(analysisEntity);

            _logger.LogInformation("Analysis {AnalysisId} successfully created.", newAnalysis.Id);

           
            var evt = new SimpleEntityCreatedEvent
            {
                EntityType = "Analysis",
                Name = newAnalysis.Name
            };
            await _publishEndpoint.Publish(evt, cancellationToken);

           
            await _publishEndpoint.Publish(new AutocompleteEntityChanged
            {
                Type = EntityType.Analysis,
                Kind = ChangeKind.Created,
                Id = newAnalysis.Id,
                Name = newAnalysis.Name
            }, cancellationToken);

            return newAnalysis.Id;
        }
    }
}
