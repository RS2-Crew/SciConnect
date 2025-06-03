using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DB.Application.Features.Employees.Commands.AddKeywordToEmploye
{
    public class AddKeywordToEmployeCommand : IRequest<Unit>
    {
            
        public int KeywordId { get; }
        public int EmployeeId { get; }

        public AddKeywordToEmployeCommand(int employeeId, int keywordId)
        {
            KeywordId = keywordId;
            EmployeeId = employeeId;
        }
    }
}
