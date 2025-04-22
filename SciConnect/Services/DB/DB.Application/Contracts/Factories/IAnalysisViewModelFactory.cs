using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Analysis.Queries.ViewModels;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Factories
{
    public interface IAnalysisViewModelFactory
    {
        AnalysisViewModel CreateViewModel(Analysis analysis);
    }
}
