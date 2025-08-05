using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Microorganisms.Queries.ViewModels;
using DB.Application.Features.Analysis.Queries.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DB.Application.Features.Analysis.Queries.GetMicroorganismWithAnalyses;
using DB.Domain.Entities;

namespace DB.Application.Features.Analysis.Queries.GetMicroorganismWithAnalyses
{
    public class GetAnalysisByMicroorganismQueryHandler : IRequestHandler<GetAnalysisByMicroorganismQuery, MicroorganismViewModel>
    {
        private readonly IMicroorganismRepository _microorganismRepository;
        private readonly IAnalysisViewModelFactory _analysisVMFactory;

        public GetAnalysisByMicroorganismQueryHandler(
            IMicroorganismRepository microorganismRepository,
            IAnalysisViewModelFactory analysisVMFactory)
        {
            _microorganismRepository = microorganismRepository;
            _analysisVMFactory = analysisVMFactory;
        }

        public async Task<MicroorganismViewModel> Handle(GetAnalysisByMicroorganismQuery request, CancellationToken cancellationToken)
        {
            var microorganism = await _microorganismRepository
                .GetQueryable()
                .Include(m => m.Analyses)
                .FirstOrDefaultAsync(m => m.Name.ToLower() == request.MicroorganismName.ToLower(), cancellationToken);

            if (microorganism == null)
                return null;

            var viewModel = new MicroorganismViewModel
            {
                Id = microorganism.Id,
                Name = microorganism.Name,
                Analyses = microorganism.Analyses
                    .Select(a => _analysisVMFactory.CreateViewModel(a))
                    .ToList()
            };

            return viewModel;
        }
    }
}
