using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DB.Application.Features.Institutions.Commands.AddMicroorganismToInstitution
{
    public class AddMicroorganismToInstitutionCommand : IRequest<Unit>
    {
        public int InstitutionId { get; }
        public int MicroorganismId { get; }

        public AddMicroorganismToInstitutionCommand(int institutionId, int microorganismId)
        {
            InstitutionId = institutionId;
            MicroorganismId = microorganismId;
        }
    }
}
