using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Analysis.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Analysis.Queries.GetAnalysisWithMicroorganism
{
    public class GetMicroorganismsByAnalysisQuery : IRequest<AnalysisViewModel>
    {
        public string AnalysisName { get; set; }

        public GetMicroorganismsByAnalysisQuery(string analysisName)
        {
            AnalysisName = analysisName;
        }
    }
}
