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
        public CreateAnalysisCommand(string name, List<string> microorganism)
        {
            Name = name;
            Microorganism = microorganism;
        }

        public string Name { get; set; }
        public List<string> Microorganism { get; set; }



    }
}
