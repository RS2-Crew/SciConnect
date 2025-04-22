using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DB.Application.Features.Analyses.Commands.CreateAnalysis;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Factories
{
    public interface IAnalysisFactory
    {
        Analysis Create(CreateAnalysisCommand command);
    }
}
