using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Microorganisms.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Analysis.Queries.GetMicroorganismWithAnalyses
{
    public class GetAnalysisByMicroorganismQuery : IRequest<MicroorganismViewModel>

    {
        public string MicroorganismName { get; set; }

        public GetAnalysisByMicroorganismQuery(string microorganismName)
        {
            MicroorganismName = microorganismName;
        }
    }
}
