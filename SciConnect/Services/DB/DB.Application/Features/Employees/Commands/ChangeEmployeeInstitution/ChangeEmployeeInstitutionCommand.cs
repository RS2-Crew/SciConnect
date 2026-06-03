using MediatR;

namespace DB.Application.Features.Employees.Commands.ChangeEmployeeInstitution
{
    public class ChangeEmployeeInstitutionCommand : IRequest<Unit>
    {
        public int EmployeeId { get; }
        public int InstitutionId { get; }

        public ChangeEmployeeInstitutionCommand(int employeeId, int institutionId)
        {
            EmployeeId = employeeId;
            InstitutionId = institutionId;
        }
    }
}
