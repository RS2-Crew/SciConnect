using System.Collections.Generic;
using DB.Application.Features.Employees.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Employees.Queries.GetListOfEmployees
{
    public class GetListOfEmployeesQuery : IRequest<IReadOnlyCollection<EmployeeViewModel>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public GetListOfEmployeesQuery(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
