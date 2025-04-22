using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Features.Keywords.Queries.ViewModels;
using DB.Application.Features.Microorganisms.Queries.ViewModels;
using DB.Domain.Entities;

namespace DB.Infrastructure.Factories
{
    public class KeywordViewModelFactory : IKeywordViewModelFactory
    {
        public KeywordViewModel CreateViewModel(Keyword keyword)
        {
            var keywordVM = new KeywordViewModel();

            keywordVM.Id = keyword.Id;
            keywordVM.Name = keyword.Name;

            return keywordVM;
        }
    }
}
