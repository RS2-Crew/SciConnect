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
    public class InstrumentViewModelFactory : IInstrumentViewModelFactory
    {
        public InstrumentViewModel CreateViewModel(Instrument instrument)
        {
            var instrumentVM = new InstrumentViewModel();

            instrumentVM.Id = instrument.Id;
            instrumentVM.Name = instrument.Name;

            return instrumentVM;
        }
    }
}
