using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using MediatR;

namespace DB.Application.Features.Analysis.Commands.AddMicroorganismToAnalysis
{
    public class AddMicroorganismToAnalysisCommandHandler : IRequestHandler<AddMicroorganismToAnalysisCommand, Unit>
    {
        private readonly IMicroorganismRepository _microorganismRepository;
        private readonly IAnalysisRepository _analysisRepository;

        public AddMicroorganismToAnalysisCommandHandler(IMicroorganismRepository microorganismRepository, IAnalysisRepository analysisRepository)
        {
            _microorganismRepository = microorganismRepository;
            _analysisRepository = analysisRepository;
        }

        public async Task<Unit> Handle(AddMicroorganismToAnalysisCommand request, CancellationToken cancellationToken)
        {
            var microorganism = await _microorganismRepository.GetByIdAsync(request.MicroorganismId);
            var analysis = await _analysisRepository.GetByIdAsync(request.AnalysisId);

            if (microorganism == null || analysis == null)
            {
                throw new ArgumentException("Invalid analysis or microorganism ID.");
            }

            analysis.AddMicroorgnaism(microorganism);
            await _analysisRepository.UpdateAsync(analysis);

            return Unit.Value;
        }
    }
}
