using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using MediatR;

namespace DB.Application.Features.Institutions.Commands.AddAnalysisToInstitution
{
    public class AddAnalysisToInstitutionCommandHandler : IRequestHandler<AddAnalysisToInstitutionCommand, Unit>
    {

        private readonly IInstitutionRepository _institutionRepository;
        private readonly IAnalysisRepository _analysisRepository;

        public AddAnalysisToInstitutionCommandHandler(IInstitutionRepository institutionRepository, IAnalysisRepository analysisRepository)
        {
            _institutionRepository = institutionRepository;
            _analysisRepository = analysisRepository;
        }

        public async Task<Unit> Handle(AddAnalysisToInstitutionCommand request, CancellationToken cancellationToken)
        {
            var institution = await _institutionRepository.GetByIdAsync(request.InstitutionId);
            var analysis = await _analysisRepository.GetByIdAsync(request.AnalysisId);

            if (institution == null || analysis == null)
            {
                throw new ArgumentException("Invalid analysis or institution ID.");
            }

            institution.AddAnalysis(analysis);
            await _institutionRepository.UpdateAsync(institution);

            return Unit.Value;
        }
    }
}
