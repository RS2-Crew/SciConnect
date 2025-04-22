using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DB.Application.Features.Microorganisms.Commands.CreateMicroorganism

{
    public class CreateMicroorganismCommand : IRequest<int>
    {
        public string Name { get; set; }
        public CreateMicroorganismCommand(string name)
        {
            Name = name;
        }
    }
}
