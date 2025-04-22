using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DB.Application.Features.Microorganisms.Commands.DeleteMicroorganism

{
    public class DeleteMicroorganismCommand : IRequest<Unit>
    {
        public string Name { get; set; }

        public DeleteMicroorganismCommand(string name)
        {
            Name = name;
        }
    }
}
