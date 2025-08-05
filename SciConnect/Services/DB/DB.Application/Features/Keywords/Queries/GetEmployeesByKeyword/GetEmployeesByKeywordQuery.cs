using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Employees.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Keywords.Queries.GetEmployeesByKeyword
{
    public class GetEmployeesByKeywordQuery : IRequest<IReadOnlyCollection<EmployeeViewModel>>
    {
        public string KeywordName { get; }

        public GetEmployeesByKeywordQuery(string keywordName)
        {
            KeywordName = keywordName;
        }
    }
}
