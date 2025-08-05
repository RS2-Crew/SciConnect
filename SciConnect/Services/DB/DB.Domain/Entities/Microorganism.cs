using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Domain.Common;

namespace DB.Domain.Entities
{
    public class Microorganism : AggregateRoot
    {
        public string Name { get; private set; }

        private readonly List<Institution> _institutions = new List<Institution>();
        public IReadOnlyCollection<Institution> Institutions => _institutions.AsReadOnly();

        private readonly List<Analysis> _analyses = new();
        public IReadOnlyCollection<Analysis> Analyses => _analyses.AsReadOnly();

        private Microorganism() { }

        public Microorganism(string name)
        {
            Name = name;
        }
        public void AddInstitution(Institution institution)
        {
            _institutions.Add(institution);
        }
        public void AddAnalysis(Analysis analysis)
        {
            if (!_analyses.Contains(analysis))
            {
                _analyses.Add(analysis);
            }
        }
    }
}
