using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Features.Institutions.Commands.CreateInstitution;
using DB.Domain.Entities;

namespace DB.Infrastructure.Factories
{
    public class InstitutionFactory : IInstitutionFactory
    {
        public Institution Create(CreateInstitutionCommand command)
        {
            var instituition = new Institution(command.Name, command.Street, command.StreetNumber, command.City, command.Country, command.Phone, command.Email, command.Website);
            
            return instituition;
        }
    }
}
