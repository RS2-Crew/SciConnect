using DB.Application.Contracts.Factories;
using DB.Application.Features.Analysis.Queries.ViewModels;
using DB.Domain.Entities;

namespace DB.Infrastructure.Factories
{
    public class AnalysisViewModelFactory : IAnalysisViewModelFactory
    {
        public AnalysisViewModel CreateViewModel(Analysis analysis)
        {
            return new AnalysisViewModel(analysis.Id, analysis.Name);
        }
    }
}
