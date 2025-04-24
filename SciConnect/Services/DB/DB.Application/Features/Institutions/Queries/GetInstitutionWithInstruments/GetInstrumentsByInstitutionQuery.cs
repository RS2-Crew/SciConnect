using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Institutions.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Institutions.Queries.GetInstitutionWithInstruments
{
public class GetInstrumentsByInstitutionQuery : IRequest<InstitutionViewModel>
    {
        public string InstitutionName { get; set; }

        public GetInstrumentsByInstitutionQuery(string institutionName)
        {
            InstitutionName = institutionName;
        }
    }
}
