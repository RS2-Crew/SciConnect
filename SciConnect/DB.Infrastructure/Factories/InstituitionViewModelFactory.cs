using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Features.Institutions.Queries.ViewModels;
using DB.Domain.Entities;

namespace DB.Infrastructure.Factories
{
    public class InstituitionViewModelFactory : IInstitutionViewModelFactory
    {
        public InstitutionViewModel CreateViewModel(Institution institution)
        {
            var institutionVM = new InstitutionViewModel();

            institutionVM.Name = institution.Name;
            institutionVM.Street = institution.Street;
            institutionVM.StreetNumber = institution.StreetNumber;
            institutionVM.City = institution.City;
            institutionVM.Country = institution.Country;
            institutionVM.Phone = institution.Phone;
            institutionVM.Email = institution.Email;
            institutionVM.Website = institution.Website;

            return institutionVM;
        }
    }
}
