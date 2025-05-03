using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Institutions.Queries.ViewModels;
using DB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MediatR;
using DB.Application.Features.Instruments.Queries.ViewModels;

namespace DB.Application.Features.Institutions.Queries.GetInstitutionWithInstruments
{
    public class GetInstrumentsByInstitutionQueryHandler : IRequestHandler<GetInstrumentsByInstitutionQuery, InstitutionViewModel>
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IInstrumentViewModelFactory _instrumentVmFactory;

        public GetInstrumentsByInstitutionQueryHandler(
            IInstitutionRepository institutionRepository,
            IInstrumentViewModelFactory instrumentVmFactory)
        {
            _institutionRepository = institutionRepository;
            _instrumentVmFactory = instrumentVmFactory;
        }

        public async Task<InstitutionViewModel> Handle(
            GetInstrumentsByInstitutionQuery request,
            CancellationToken cancellationToken)
        {
            
            var institution = await _institutionRepository
               .GetQueryable()
               .Include(i => i.Instruments)
               .FirstOrDefaultAsync(i => i.Name.ToLower() == request.InstitutionName.ToLower(), cancellationToken);

            if (institution == null)
                return null;

            Console.WriteLine(institution.Instruments.Count);

            var viewModel = new InstitutionViewModel
            {
                Id = institution.Id,
                Name = institution.Name,
                Street = institution.Street,
                StreetNumber = institution.StreetNumber,
                City = institution.City,
                Country = institution.Country,
                Phone = institution.Phone,
                Email = institution.Email,
                Website = institution.Website,
                Instruments = institution.Instruments
                            .Select(i => new InstrumentBasicViewModel
                            {
                                Id = i.Id,
                                Name = i.Name
                            })
                            .ToList()
            };

            return viewModel;
        }
    }
}
