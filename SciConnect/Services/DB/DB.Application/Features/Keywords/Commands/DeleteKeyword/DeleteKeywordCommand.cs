using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DB.Application.Features.Keywords.Commands.DeleteKeyword
{
    public class DeleteKeywordCommand : IRequest<Unit>
    {
        public string Name { get; set; }

        public DeleteKeywordCommand(string name)
        {
            Name = name;
        }
    }
}
