using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Features.Instruments.Commands.CreateInstrument;
using DB.Domain.Entities;

namespace DB.Infrastructure.Factories
{
    public class InstrumentFactory : IInstrumentFactory
    {
        public Instrument Create(CreateInstrumentCommand command)
        {
            return new Instrument(command.Name);
        }

    }
}
