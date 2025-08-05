using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Employees.Queries.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DB.Application.Features.Keywords.Queries.GetEmployeesByKeyword
{
    public class GetEmployeesByKeywordQueryHandler : IRequestHandler<GetEmployeesByKeywordQuery, IReadOnlyCollection<EmployeeViewModel>>
    {
        private readonly IKeywordRepository _keywordRepository;
        private readonly IEmployeeViewModelFactory _viewModelFactory;

        public GetEmployeesByKeywordQueryHandler(
            IKeywordRepository keywordRepository,
            IEmployeeViewModelFactory viewModelFactory)
        {
            _keywordRepository = keywordRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<IReadOnlyCollection<EmployeeViewModel>> Handle(GetEmployeesByKeywordQuery request, CancellationToken cancellationToken)
        {
            var keyword = await _keywordRepository
                .GetQueryable()
                .Include(k => k.Employees)
                .FirstOrDefaultAsync(k => k.Name.ToLower() == request.KeywordName.ToLower(), cancellationToken);

            if (keyword == null || !keyword.Employees.Any())
            {
                return new List<EmployeeViewModel>();
            }

            return keyword.Employees
                          .Select(_viewModelFactory.CreateViewModel)
                          .ToList();
        }
    }
}
