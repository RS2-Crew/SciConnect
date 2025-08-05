using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DB.Application.Features.Analysis.Commands.AddMicroorganismToAnalysis
{
    public class AddMicroorganismToAnalysisCommand : IRequest<Unit>
    {
        public int AnalysisId { get; }
        public int MicroorganismId { get; }

        public AddMicroorganismToAnalysisCommand(int analysisId, int microorganismId)
        {
            AnalysisId = analysisId;
            MicroorganismId = microorganismId;
        }
    }
}
