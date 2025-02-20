using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Domain.Common;

namespace DB.Domain.Entities
{
    public class Instrument : AggregateRoot
    {
        public string Name { get; private set; }

        private readonly List<Institution> _institutions = new List<Institution>();
        public IReadOnlyCollection<Institution> Institutions => _institutions.AsReadOnly();

        private Instrument() { }

        public Instrument(string name)
        {
            Name = name;
        }

        public void AddInstitution(Institution institution)
        {
            _institutions.Add(institution);
        }
    }
}
