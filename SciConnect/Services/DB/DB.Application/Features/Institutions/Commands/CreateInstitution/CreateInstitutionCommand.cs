using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DB.Application.Features.Institutions.Commands.CreateInstitution
{
    public class CreateInstitutionCommand : IRequest<int>
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Street { get; private set; }
        public string StreetNumber { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }
        public string? Phone { get; private set; }
        public string? Email { get; private set; }
        public string? Website { get; private set; }
    }
}
