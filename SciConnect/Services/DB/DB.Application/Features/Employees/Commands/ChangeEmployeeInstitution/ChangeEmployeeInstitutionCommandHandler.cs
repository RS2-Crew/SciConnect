using System;
using System.Threading;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using MediatR;

namespace DB.Application.Features.Employees.Commands.ChangeEmployeeInstitution
{
    public class ChangeEmployeeInstitutionCommandHandler : IRequestHandler<ChangeEmployeeInstitutionCommand, Unit>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IInstitutionRepository _institutionRepository;

        public ChangeEmployeeInstitutionCommandHandler(
            IEmployeeRepository employeeRepository,
            IInstitutionRepository institutionRepository)
        {
            _employeeRepository = employeeRepository;
            _institutionRepository = institutionRepository;
        }

        public async Task<Unit> Handle(ChangeEmployeeInstitutionCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            var institution = await _institutionRepository.GetByIdAsync(request.InstitutionId);

            if (employee == null || institution == null)
            {
                throw new ArgumentException("Invalid employee or institution ID.");
            }

            employee.ChangeInstitution(institution);
            await _employeeRepository.UpdateAsync(employee);

            return Unit.Value;
        }
    }
}
