using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Institutions.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Instruments.Queries.GetInstitutionsByInstrument
{
    public class GetInstitutionsByInstrumentQuery : IRequest<IReadOnlyCollection<InstitutionViewModel>>
    {
        public string InstrumentName { get; }

        public GetInstitutionsByInstrumentQuery(string instrumentName)
        {
            InstrumentName = instrumentName;
        }
    }
}
