using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using MediatR;

namespace DB.Application.Features.Institutions.Commands.AddInstrument
{
    public class AddInstrumentToInstitutionCommandHandler : IRequestHandler<AddInstrumentToInstitutionCommand, Unit>
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IInstrumentRepository _instrumentRepository;

        public AddInstrumentToInstitutionCommandHandler(
            IInstitutionRepository institutionRepository,
            IInstrumentRepository instrumentRepository)
        {
            _institutionRepository = institutionRepository;
            _instrumentRepository = instrumentRepository;
        }

        public async Task<Unit> Handle(AddInstrumentToInstitutionCommand request, CancellationToken cancellationToken)
        {
            var institution = await _institutionRepository.GetByIdAsync(request.InstitutionId);
            var instrument = await _instrumentRepository.GetByIdAsync(request.InstrumentId);

            if (institution == null || instrument == null)
            {
                throw new ArgumentException("Invalid instrument or institution ID.");
            }

            institution.AddInstrument(instrument);
            await _institutionRepository.UpdateAsync(institution);

            return Unit.Value;
        }
    }
}
