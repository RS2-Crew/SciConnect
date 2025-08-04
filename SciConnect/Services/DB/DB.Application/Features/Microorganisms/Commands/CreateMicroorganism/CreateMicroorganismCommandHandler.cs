using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Domain.Entities;
using EventBus.Messages.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Microorganisms.Commands.CreateMicroorganism
{
    class CreateMicroorganismCommandHandler : IRequestHandler<CreateMicroorganismCommand, int>
    {
        private readonly IMicroorganismRepository _microorganismRepository;
        private readonly IMicroorganismFactory _microorganismFactory;
        private readonly ILogger<CreateMicroorganismCommandHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateMicroorganismCommandHandler(IMicroorganismRepository microorganismRepository, IMicroorganismFactory microorganismFactory, ILogger<CreateMicroorganismCommandHandler> logger, IPublishEndpoint publishEndpoint)
        {
            _microorganismRepository = microorganismRepository ?? throw new ArgumentNullException(nameof(microorganismRepository));
            _microorganismFactory = microorganismFactory ?? throw new ArgumentNullException(nameof(microorganismFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<int> Handle(CreateMicroorganismCommand request, CancellationToken cancellationToken)
        {
            var microorganism = _microorganismFactory.Create(request);

            await _microorganismRepository.AddAsync(microorganism);

            var evt = new SimpleEntityCreatedEvent
            {
                EntityType = "Microorganism",
                Name = microorganism.Name
            };

            await _publishEndpoint.Publish(evt);

            return microorganism.Id;
        }
    }
}
