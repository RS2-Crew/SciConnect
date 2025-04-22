using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DB.Application.Features.Analysis.Queries.ViewModels;
using MediatR;
using System.Collections.Generic;

namespace DB.Application.Features.Analysis.Queries.GetListOfAnalysis
{
    public class GetListOfAnalysisQuery : IRequest<IReadOnlyCollection<AnalysisViewModel>>
    {
        public string Name { get; set; }

        public GetListOfAnalysisQuery(string name)
        {
            Name = name;
        }
    }
}