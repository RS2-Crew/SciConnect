using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace DB.Application.Features.Analyses.Commands.DeleteAnalysis
{
    public class DeleteAnalysisCommand : IRequest<Unit>
    {
        public string Name { get; set; }

        public DeleteAnalysisCommand(string name)
        {
            Name = name;
        }
    }
}