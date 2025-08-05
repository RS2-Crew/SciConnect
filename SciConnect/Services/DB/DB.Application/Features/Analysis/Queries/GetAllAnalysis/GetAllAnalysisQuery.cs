using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DB.Application.Features.Analysis.Queries.ViewModels;
using MediatR;


namespace DB.Application.Features.Analyses.Queries.GetAllAnalyses
{
    public class GetAllAnalysesQuery : IRequest<IReadOnlyList<AnalysisViewModel>>
    {
    }
}
