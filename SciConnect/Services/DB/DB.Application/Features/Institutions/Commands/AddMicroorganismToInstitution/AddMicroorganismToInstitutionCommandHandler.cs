using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using MediatR;

namespace DB.Application.Features.Institutions.Commands.AddMicroorganismToInstitution
{
    public class AddMicroorganismToInstitutionCommandHandler : IRequestHandler<AddMicroorganismToInstitutionCommand, Unit>
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IMicroorganismRepository _microorganismRepository;

        public AddMicroorganismToInstitutionCommandHandler(IInstitutionRepository institutionRepository, IMicroorganismRepository microorganismRepository)
        {
            _institutionRepository = institutionRepository;
            _microorganismRepository = microorganismRepository;
        }

        public async Task<Unit> Handle(AddMicroorganismToInstitutionCommand request, CancellationToken cancellationToken)
        {
            var institution = await _institutionRepository.GetByIdAsync(request.InstitutionId);
            var microorganism = await _microorganismRepository.GetByIdAsync(request.MicroorganismId);

            if (institution == null || microorganism == null)
            {
                throw new ArgumentException("Invalid microorganism or institution ID.");
            }

            institution.AddMicroorganism(microorganism);
            await _institutionRepository.UpdateAsync(institution);

            return Unit.Value;
        }
    }
}
