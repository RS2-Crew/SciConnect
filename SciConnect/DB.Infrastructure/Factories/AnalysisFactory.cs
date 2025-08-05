using DB.Application.Contracts.Factories;
using DB.Application.Features.Analyses.Commands.CreateAnalysis;
using DB.Domain.Entities;

namespace DB.Infrastructure.Factories
{
    public class AnalysisFactory : IAnalysisFactory
    {
        public Analysis Create(CreateAnalysisCommand command)
        {
            return new Analysis(command.Name);
        }
    }
}
