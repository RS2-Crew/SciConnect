using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Keywords.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Keywords.Queries.GetAllKeywords
{
    public class GetAllKeywordsQuery : IRequest<IReadOnlyList<KeywordViewModel>>
    {
    }
}
