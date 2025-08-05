using System.Threading;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Employees.Commands.CreateEmployee;
using DB.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, int>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IEmployeeFactory _employeeFactory;
        private readonly ILogger<CreateEmployeeCommandHandler> _logger;

        public CreateEmployeeCommandHandler(
            IEmployeeRepository employeeRepository,
            IInstitutionRepository institutionRepository,
            IEmployeeFactory employeeFactory,
            ILogger<CreateEmployeeCommandHandler> logger)
        {
            _employeeRepository = employeeRepository;
            _institutionRepository = institutionRepository;
            _employeeFactory = employeeFactory;
            _logger = logger;
        }

        public async Task<int> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var institution = await _institutionRepository.GetByIdAsync(request.InstitutionId);
            if (institution == null)
            {
                _logger.LogError("Institution with ID {Id} not found", request.InstitutionId);
                throw new ArgumentException($"Institution with ID {request.InstitutionId} not found.");
            }

            var employee = _employeeFactory.Create(request, institution);
            await _employeeRepository.AddAsync(employee);

            return employee.Id;
        }
    }
}
