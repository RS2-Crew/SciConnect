using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Institutions.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Institutions.Queries.GetInstitutionWithEmployees
{
    public class GetEmployeesByInstitutionQuery : IRequest<InstitutionViewModel>
    {
        public string InstitutionName { get; set; }

        public GetEmployeesByInstitutionQuery(string institutionName)
        {
            InstitutionName = institutionName;
        }
    }
}
