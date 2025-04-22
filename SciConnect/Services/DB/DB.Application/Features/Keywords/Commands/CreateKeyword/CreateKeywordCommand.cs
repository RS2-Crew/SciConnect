using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DB.Application.Features.Keywords.Commands.CreateKeyword
{
    public class CreateKeywordCommand : IRequest<int>
    {
        public string Name { get; set; }

        public CreateKeywordCommand(string name)
        {
            Name = name;
        }
    }
}

