using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Features.Instruments.Queries.ViewModels;
using DB.Application.Features.Microorganisms.Queries.ViewModels;
using DB.Domain.Entities;

namespace DB.Infrastructure.Factories
{
    public class MicroorganismViewModelFactory : IMicroorganismViewModelFactory
    {
        public MicroorganismViewModel CreateViewModel(Microorganism microorganism)
        {
            var microorganismVM = new MicroorganismViewModel();

            microorganismVM.Id = microorganism.Id;
            microorganismVM.Name = microorganism.Name;

            return microorganismVM;
        }
    }
}
