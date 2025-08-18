using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using EventBus.Messages.Entities;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Institutions.Commands.CreateInstitution
{
    public class CreateInstitutionCommandHandler : IRequestHandler<CreateInstitutionCommand, int>
    {
        private readonly IInstitutionRepository _repository;
        private readonly IInstitutionFactory _factory;
        private readonly ILogger<CreateInstitutionCommandHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateInstitutionCommandHandler(IInstitutionRepository repository, IInstitutionFactory factory, ILogger<CreateInstitutionCommandHandler> logger, IPublishEndpoint publishEndpoint)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<int> Handle(CreateInstitutionCommand request, CancellationToken cancellationToken)
        {
            var institutionEntity = _factory.Create(request);
            var newInstitution = await _repository.AddAsync(institutionEntity);

            _logger.LogInformation($"Institution {newInstitution.Id} is successfully created.");

            var evt = new InstitutionCreatedEvent
            {
                Id = newInstitution.Id,
                Name = newInstitution.Name,
                Street = newInstitution.Street,
                StreetNumber = newInstitution.StreetNumber,
                City = newInstitution.City,
                Country = newInstitution.Country,
                Phone = newInstitution.Phone,
                Email = newInstitution.Email,
                Website = newInstitution.Website

            };

            await _publishEndpoint.Publish(evt);

            return newInstitution.Id;
        }
    }
}
