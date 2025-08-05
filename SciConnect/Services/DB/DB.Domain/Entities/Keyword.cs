using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Domain.Common;

namespace DB.Domain.Entities
{
    public class Keyword : AggregateRoot
    {
        public string Name { get; private set; }

        private readonly List<Institution> _institutions = new List<Institution>();
        public IReadOnlyCollection<Institution> Institutions => _institutions.AsReadOnly();

        private readonly List<Employee> _employees = new List<Employee>();
        public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();

        private Keyword () { }

        public Keyword(string name)
        {
            Name = name;
        }

        public void AddInstitution(Institution institution)
        {
            if (!_institutions.Contains(institution))
            {
                _institutions.Add(institution);
            }
        }

        public void AddEmployee(Employee employee)
        {
            if (!_employees.Contains(employee))
            {
                _employees.Add(employee);
            }
        }

    }
}
