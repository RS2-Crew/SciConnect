using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DB.Domain.Common;

namespace DB.Domain.Entities
{
    public class Analysis : AggregateRoot
    {
        public string Name { get; private set; }

        private readonly List<Institution> _institutions = new List<Institution>();
        public IReadOnlyCollection<Institution> Institutions => _institutions.AsReadOnly();


        private readonly List<Microorganism> _microorganisms = new List<Microorganism>();
        public IReadOnlyCollection<Microorganism> Microorganisms => _microorganisms.AsReadOnly();

        private Analysis() { }

        public Analysis(string name)
        {
            Name = name;
        }

        public void AddInstitution(Institution institution)
        {
            _institutions.Add(institution);
        }

        public void AddMicroorganism(Microorganism microorganism)
        {
            if (!_microorganisms.Contains(microorganism))
            {
                _microorganisms.Add(microorganism);
            }
        }
    }
}
