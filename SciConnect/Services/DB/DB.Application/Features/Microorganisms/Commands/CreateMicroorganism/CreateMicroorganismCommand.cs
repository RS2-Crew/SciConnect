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

        public List<string> Analisys { get; set; }

        public CreateMicroorganismCommand(string name, List<string> analisys)
        {
            Name = name;
            Analisys = analisys;
        }
    }
}
