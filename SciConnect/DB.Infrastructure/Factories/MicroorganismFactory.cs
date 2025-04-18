using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Features.Microorganisms.Commands.CreateMicroorganism;
using DB.Domain.Entities;

namespace DB.Infrastructure.Factories
{
    public class MicroorganismFactory : IMicroorganismFactory
    {
        public Microorganism Create(CreateMicroorganismCommand command)
        {
            return new Microorganism(command.Name);
        }
    }
}
