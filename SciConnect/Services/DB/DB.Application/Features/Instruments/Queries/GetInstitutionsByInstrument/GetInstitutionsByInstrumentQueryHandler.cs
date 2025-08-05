using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Institutions.Queries.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DB.Application.Features.Instruments.Queries.GetInstitutionsByInstrument
{
    public class GetInstitutionsByInstrumentQueryHandler : IRequestHandler<GetInstitutionsByInstrumentQuery, IReadOnlyCollection<InstitutionViewModel>>
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstitutionViewModelFactory _viewModelFactory;

        public GetInstitutionsByInstrumentQueryHandler(
            IInstrumentRepository instrumentRepository,
            IInstitutionViewModelFactory viewModelFactory)
        {
            _instrumentRepository = instrumentRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<IReadOnlyCollection<InstitutionViewModel>> Handle(GetInstitutionsByInstrumentQuery request, CancellationToken cancellationToken)
        {
            var instrument = await _instrumentRepository
                .GetQueryable()
                .Include(i => i.Institutions)
                .FirstOrDefaultAsync(i => i.Name.ToLower() == request.InstrumentName.ToLower(), cancellationToken);

            if (instrument == null || !instrument.Institutions.Any())
            {
                return new List<InstitutionViewModel>();
            }

            return instrument.Institutions
                            .Select(_viewModelFactory.CreateViewModel)
                            .ToList();
        }

    }
}
