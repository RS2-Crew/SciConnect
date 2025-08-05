using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DB.Application.Features.Instruments.Commands.CreateInstrument
{
    public class CreateInstrumentCommand : IRequest<int>
    {
        public string Name { get; set; }
        public CreateInstrumentCommand(string name)
        {
            Name = name;
        }
    }
}
