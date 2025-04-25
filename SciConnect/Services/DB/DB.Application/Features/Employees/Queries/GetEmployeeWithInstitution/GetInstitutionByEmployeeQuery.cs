using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Employees.Queries.ViewModels;
using MediatR;
namespace DB.Application.Features.Employees.Queries.GetEmployeeWithInstitution
{
    public class GetInstitutionByEmployeeQuery : IRequest<EmployeeViewModel>
    {

        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }

        public GetInstitutionByEmployeeQuery(string employeeFirstName, string employeeLastName)
        {
            EmployeeFirstName=employeeFirstName;
            EmployeeLastName=employeeLastName;
        }
    }
}
