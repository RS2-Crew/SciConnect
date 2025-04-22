using System;
using System.Threading;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Employees.Commands.DeleteEmployee;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Employees.Commands.DeleteEmployee
{
    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Unit>
    {
        private readonly IEmployeeRepository _repository;
        private readonly ILogger<DeleteEmployeeCommandHandler> _logger;

        public DeleteEmployeeCommandHandler(IEmployeeRepository repository, ILogger<DeleteEmployeeCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
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

            return Unit.Value;
        }
    }
}
