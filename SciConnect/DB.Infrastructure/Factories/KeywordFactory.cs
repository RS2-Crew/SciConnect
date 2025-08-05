using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Features.Keywords.Commands.CreateKeyword;
using DB.Domain.Entities;

namespace DB.Infrastructure.Factories
{
    public class KeywordFactory : IKeywordFactory
    {
        public Keyword Create(CreateKeywordCommand command)
        {
            return new Keyword(command.Name);
        }
    }
}
