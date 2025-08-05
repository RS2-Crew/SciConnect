using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DB.Application.Features.Institutions.Commands.AddAnalysisToInstitution
{
    public class AddAnalysisToInstitutionCommand : IRequest<Unit>
    {
        public int InstitutionId { get; }
        public int AnalysisId { get; }

        public AddAnalysisToInstitutionCommand(int institutionId, int analysisId)
        {
            InstitutionId = institutionId;
            AnalysisId = analysisId;
        }
    }
}
