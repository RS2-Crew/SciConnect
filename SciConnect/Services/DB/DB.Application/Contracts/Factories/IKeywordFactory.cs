using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Keywords.Commands.CreateKeyword;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Factories
{
    public interface IKeywordFactory
    {
        Keyword Create(CreateKeywordCommand command);
    }
}
