using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DB.Application.Features.Institutions.Commands.AddInstrument
{
    public class AddInstrumentToInstitutionCommand : IRequest<Unit>
    {
        public int InstitutionId { get; }
        public int InstrumentId { get; }

        public AddInstrumentToInstitutionCommand(int institutionId, int instrumentId)
        {
            InstitutionId = institutionId;
            InstrumentId = instrumentId;
        }
    }
}
