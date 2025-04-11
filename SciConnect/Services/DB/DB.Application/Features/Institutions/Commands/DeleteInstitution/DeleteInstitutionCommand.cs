using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DB.Application.Features.Institutions.Commands.DeleteInstitution
{
    public class DeleteInstitutionCommand : IRequest<Unit>
    {
        public string Name { get; }

        public DeleteInstitutionCommand(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }

}
