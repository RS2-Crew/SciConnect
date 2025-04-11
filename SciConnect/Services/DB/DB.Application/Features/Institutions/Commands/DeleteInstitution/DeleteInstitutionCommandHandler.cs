using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using DB.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using MediatR;

namespace DB.Application.Features.Institutions.Commands.DeleteInstitution
{
    public class DeleteInstitutionCommandHandler : IRequestHandler<DeleteInstitutionCommand, Unit>
    {
        private readonly IInstitutionRepository _repository;
        private readonly ILogger<DeleteInstitutionCommandHandler> _logger;

        public DeleteInstitutionCommandHandler(IInstitutionRepository repository, ILogger<DeleteInstitutionCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(DeleteInstitutionCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                _logger.LogError("Institution name must be provided.");
                throw new ArgumentException("Institution name must be provided.");
            }

            var institution = await _repository.GetByNameAsync(request.Name);



            if (institution == null)
            {
                _logger.LogError($"Institution with name {request.Name} not found.");
                throw new ArgumentException($"Institution with name {request.Name} not found.");
            }

            await _repository.DeleteAsync(institution);

            _logger.LogInformation($"Institution with name {request.Name} successfully deleted.");

            return Unit.Value;
        }
    }
}
