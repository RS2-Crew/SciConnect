using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace DB.Application.Features.Analyses.Commands.CreateAnalysis
{
    public class CreateAnalysisCommand : IRequest<int>
    {
        public string Name { get; private set; }

        public CreateAnalysisCommand(string name)
        {
            Name = name;
        }
    }
}
