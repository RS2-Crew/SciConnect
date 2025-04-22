using System;

namespace DB.Application.Features.Employees.Queries.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public EmployeeViewModel(int id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }

        public EmployeeViewModel()
        {
        }
    }
}
