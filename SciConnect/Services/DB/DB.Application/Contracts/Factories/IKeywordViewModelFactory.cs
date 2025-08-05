using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Keywords.Queries.ViewModels;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Factories
{
    public interface IKeywordViewModelFactory
    {
        KeywordViewModel CreateViewModel(Keyword keyword);

    }
}
