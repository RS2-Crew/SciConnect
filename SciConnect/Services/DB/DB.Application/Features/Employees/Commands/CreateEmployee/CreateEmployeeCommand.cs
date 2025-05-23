using System;
using MediatR;

namespace DB.Application.Features.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommand : IRequest<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int InstitutionId { get; set; }

        public CreateEmployeeCommand(string firstName, string lastName, int institutionId)
        {
            FirstName = firstName;
            LastName = lastName;
            InstitutionId = institutionId;
        }
    }
}
