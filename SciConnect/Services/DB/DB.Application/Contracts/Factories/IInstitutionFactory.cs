using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Institutions.Commands.CreateInstitution;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Factories
{
    public interface IInstitutionFactory
    {
        Institution Create(CreateInstitutionCommand command);
    }
}
