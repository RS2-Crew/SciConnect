using System;
using System.Threading;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Keywords.Commands.DeleteKeyword
{
    public class DeleteKeywordCommandHandler : IRequestHandler<DeleteKeywordCommand, Unit>
    {
        private readonly IKeywordRepository _repository;
        private readonly ILogger<DeleteKeywordCommandHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public DeleteKeywordCommandHandler(
            IKeywordRepository repository,
            ILogger<DeleteKeywordCommandHandler> logger,
            IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Unit> Handle(DeleteKeywordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Keyword name must be provided.");

            var keyword = await _repository.GetByNameAsync(request.Name.Trim());
            if (keyword == null)
                throw new ArgumentException($"Keyword with name {request.Name} not found.");

            await _repository.DeleteAsync(keyword);

            _logger.LogInformation($"Keyword with name {request.Name} successfully deleted.");

            await _publishEndpoint.Publish(new AutocompleteEntityChanged
            {
                Type = EntityType.Keyword,
                Kind = ChangeKind.Deleted,
                Id = keyword.Id,
                Name = null
            });

            return Unit.Value;
        }
    }
}
