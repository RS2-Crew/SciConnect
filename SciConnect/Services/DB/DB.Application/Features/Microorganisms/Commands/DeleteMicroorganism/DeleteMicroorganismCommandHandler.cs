using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using DB.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Microorganisms.Commands.DeleteMicroorganism
{
    public class DeleteMicroorganismCommandHandler : IRequestHandler<DeleteMicroorganismCommand, Unit>
    {
        private readonly IMicroorganismRepository _repository;
        private readonly ILogger<DeleteMicroorganismCommandHandler> _logger;

        public DeleteMicroorganismCommandHandler(IMicroorganismRepository repository, ILogger<DeleteMicroorganismCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteMicroorganismCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                _logger.LogError("Microorganism name must be provided.");
                throw new ArgumentException("Microorganism name must be provided.");
            }

            var microorganism = await _repository.GetByNameAsync(request.Name.Trim());

            if (microorganism == null)
            {
                _logger.LogError($"Microorganism with name {request.Name} not found.");
                throw new ArgumentException($"Microorganism with name {request.Name} not found.");
            }

            await _repository.DeleteAsync(microorganism);

            _logger.LogInformation($"Microorganism with name {request.Name} successfully deleted.");

            return Unit.Value;
        }
    }
}
