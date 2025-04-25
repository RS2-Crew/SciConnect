using System;
using DB.Application.Features.Institutions.Queries.ViewModels;
using DB.Application.Features.Keywords.Queries.ViewModels;

namespace DB.Application.Features.Employees.Queries.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public InstitutionViewModel Institution { get; set; }

        public IEnumerable<KeywordViewModel> Keywords { get; set; }

        public EmployeeViewModel(int id, string firstName, string lastName, InstitutionViewModel institution)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Institution = institution;
        }
        public EmployeeViewModel() { }


    }
}
