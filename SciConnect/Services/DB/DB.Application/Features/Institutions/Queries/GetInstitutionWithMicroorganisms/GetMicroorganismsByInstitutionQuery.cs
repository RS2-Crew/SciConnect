using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Institutions.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Institutions.Queries.GetInstitutionWithMicroorganisms
{
    public class GetMicroorganismsByInstitutionQuery : IRequest<InstitutionViewModel>
    {
        public string InstitutionName { get; set; }

        public GetMicroorganismsByInstitutionQuery(string institutionName)
        {
            InstitutionName = institutionName;
        }
    }
}
