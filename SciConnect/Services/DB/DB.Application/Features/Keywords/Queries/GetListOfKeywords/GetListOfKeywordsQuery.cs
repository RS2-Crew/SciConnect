using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Keywords.Queries.ViewModels;
using DB.Application.Features.Microorganisms.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Keywords.Queries.GetListOfKeywords
{
    public class GetListOfKeywordsQuery : IRequest<IReadOnlyCollection<KeywordViewModel>>
    {
        public string Name { get; set; }

        public GetListOfKeywordsQuery(string name)
        {
            Name = name;
        }
    }
}
