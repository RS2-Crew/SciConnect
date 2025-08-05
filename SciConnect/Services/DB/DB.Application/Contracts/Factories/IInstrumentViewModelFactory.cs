using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Instruments.Queries.ViewModels;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Factories
{
    public interface IInstrumentViewModelFactory
    {
        InstrumentViewModel CreateViewModel(Instrument instrument);
    }
}
