using System;
using System.Threading;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Employees.Commands.DeleteEmployee;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Employees.Commands.DeleteEmployee
{
    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Unit>
    {
        private readonly IEmployeeRepository _repository;
        private readonly ILogger<DeleteEmployeeCommandHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public DeleteEmployeeCommandHandler(IEmployeeRepository repository, ILogger<DeleteEmployeeCommandHandler> logger, IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = await _repository.GetByIdAsync(request.Id);

            if (employee == null)
            {
                _logger.LogError($"Employee with ID {request.Id} not found.");
                throw new ArgumentException($"Employee with ID {request.Id} not found.");
            }

            await _repository.DeleteAsync(employee);

            _logger.LogInformation($"Employee with ID {request.Id} successfully deleted.");

            await _publishEndpoint.Publish(new AutocompleteEntityChanged
            {
                Type = EntityType.Employee,
                Kind = ChangeKind.Deleted,
                Id = request.Id,
                Name = null
            });

            return Unit.Value;
        }
    }
}
