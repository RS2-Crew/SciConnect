using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Institutions.Commands.CreateInstitution
{
    public class CreateInstitutionCommandHandler : IRequestHandler<CreateInstitutionCommand, int>
    {
        private readonly IInstitutionRepository _repository;
        private readonly IInstitutionFactory _factory;
        private readonly ILogger<CreateInstitutionCommandHandler> _logger;

        public CreateInstitutionCommandHandler(IInstitutionRepository repository, IInstitutionFactory factory, ILogger<CreateInstitutionCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(CreateInstitutionCommand request, CancellationToken cancellationToken)
        {
            var institutionEntity = _factory.Create(request);
            var newInstitution = await _repository.AddAsync(institutionEntity);

            _logger.LogInformation($"Institution {newInstitution.Id} is successfully created.");

            return newInstitution.Id;
        }
    }
}
