using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Institutions.Queries.ViewModels;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Factories
{
    public interface IInstitutionViewModelFactory
    {
        InstitutionViewModel CreateViewModel(Institution institution);
    }
}
