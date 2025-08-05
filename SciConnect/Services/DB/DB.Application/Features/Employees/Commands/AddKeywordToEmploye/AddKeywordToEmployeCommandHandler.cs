using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using MediatR;

namespace DB.Application.Features.Employees.Commands.AddKeywordToEmploye
{
    public class AddKeywordToEmployeCommandHandler : IRequestHandler<AddKeywordToEmployeCommand, Unit>
    {


        private readonly IKeywordRepository _keywordRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public AddKeywordToEmployeCommandHandler(IKeywordRepository keywordRepository, IEmployeeRepository employeeRepository)
        {
            _keywordRepository = keywordRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<Unit> Handle(AddKeywordToEmployeCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            var keyword = await _keywordRepository.GetByIdAsync(request.KeywordId);

            if (employee == null || keyword == null)
            {
                throw new ArgumentException("Invalid employee or keyword ID.");
            }

            employee.AddKeyword(keyword);
            await _employeeRepository.UpdateAsync(employee);

            return Unit.Value;
        }
    }
}
