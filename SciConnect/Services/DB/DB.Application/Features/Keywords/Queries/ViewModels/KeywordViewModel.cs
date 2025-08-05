using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Employees.Queries.ViewModels;

namespace DB.Application.Features.Keywords.Queries.ViewModels
{
    public class KeywordViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<EmployeeViewModel> Employees { get; set; }

        public KeywordViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public KeywordViewModel()
        {
        }
    }
}
