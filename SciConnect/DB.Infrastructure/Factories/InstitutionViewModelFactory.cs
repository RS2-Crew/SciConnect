using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Features.Institutions.Queries.ViewModels;
using DB.Application.Features.Instruments.Queries.ViewModels;
using DB.Domain.Entities;

namespace DB.Infrastructure.Factories
{
    public class InstitutionViewModelFactory : IInstitutionViewModelFactory
    {
        public InstitutionViewModel CreateViewModel(Institution institution)
        {
            return new InstitutionViewModel
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
                Instruments = institution.Instruments.Select(instr => new InstrumentBasicViewModel
                {
                    Id = instr.Id,
                    Name = instr.Name
                }).ToList()
            };
        }
    }
}
