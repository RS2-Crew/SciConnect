using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;

using MediatR;
using DB.Application.Features.Analysis.Queries.ViewModels;


namespace DB.Application.Features.Analysis.Queries.GetListOfAnalysis
{
    public class GetListOfAnalysisQueryHandler : IRequestHandler<GetListOfAnalysisQuery, IReadOnlyCollection<AnalysisViewModel>>
    {
        private readonly IAnalysisRepository _repository;
        private readonly IAnalysisViewModelFactory _factory;

        public GetListOfAnalysisQueryHandler(IAnalysisRepository repository, IAnalysisViewModelFactory factory)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<IReadOnlyCollection<AnalysisViewModel>> Handle(GetListOfAnalysisQuery request, CancellationToken cancellationToken)
        {
            var analyses = await _repository.GetAnalysisByName(request.Name);
            return analyses
                .Select(a => _factory.CreateViewModel(a))
                .ToList()
                .AsReadOnly();
        }
    }
}
