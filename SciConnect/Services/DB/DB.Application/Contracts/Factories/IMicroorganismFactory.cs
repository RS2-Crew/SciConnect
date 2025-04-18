using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Microorganisms.Commands.CreateMicroorganism;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Factories
{
    public interface IMicroorganismFactory
    {
        Microorganism Create(CreateMicroorganismCommand command);

    }
}
