using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Keywords.Commands.CreateKeyword;
using EventBus.Messages.Entities;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Keywords.Commands.CreateKeyword
{
    public class CreateKeywordCommandHandler : IRequestHandler<CreateKeywordCommand, int>
    {
        private readonly IKeywordRepository _keywordRepository;
        private readonly IKeywordFactory _keywordFactory;
        private readonly ILogger<CreateKeywordCommandHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateKeywordCommandHandler(IKeywordRepository keywordRepository, IKeywordFactory keywordFactory, ILogger<CreateKeywordCommandHandler> logger, IPublishEndpoint publishEndpoint)
        {
            _keywordRepository = keywordRepository ?? throw new ArgumentNullException(nameof(keywordRepository));
            _keywordFactory = keywordFactory ?? throw new ArgumentNullException(nameof(keywordFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<int> Handle(CreateKeywordCommand request, CancellationToken cancellationToken)
        {
            var keyword = _keywordFactory.Create(request);

            await _keywordRepository.AddAsync(keyword);

            var evt = new SimpleEntityCreatedEvent
            {
                EntityType = "Keyword",
                Name = keyword.Name
            };

            await _publishEndpoint.Publish(evt);

            return keyword.Id;
        }
    }
}
