using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Institutions.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Institutions.Queries.GetInstitutionWithAnalyses
{
    public class GetAnalysesByInstitutionQuery : IRequest<InstitutionViewModel>
    {
        public string InstitutionName { get; set; }

        public GetAnalysesByInstitutionQuery(string institutionName)
        {
            InstitutionName = institutionName;
        }
    }
}
