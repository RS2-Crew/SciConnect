using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DB.Application.Features.Instruments.Commands.DeleteInstrument
{
    public class DeleteInstrumentCommand : IRequest<Unit>
    {
        public string Name { get; set; }

        public DeleteInstrumentCommand(string name)
        {
            Name = name;
        }
    }
}
