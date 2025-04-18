using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Microorganisms.Queries.ViewModels;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Factories
{
    public interface IMicroorganismViewModelFactory
    {
        MicroorganismViewModel CreateViewModel(Microorganism microorganism);
    }
}
