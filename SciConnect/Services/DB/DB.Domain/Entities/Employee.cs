using System;
using System;
using System.Collections.Generic;
using DB.Domain.Common;

namespace DB.Domain.Entities
{
    public class Employee : AggregateRoot
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public Institution Institution { get; private set; }

        private readonly List<Keyword> _keywords = new();
        public IReadOnlyCollection<Keyword> Keywords => _keywords.AsReadOnly();

        private Employee() { }

        public Employee(string firstName, string lastName, Institution institution)
        {
            FirstName = firstName;
            LastName = lastName;
            Institution = institution ?? throw new ArgumentNullException(nameof(institution));
        }

        public void AddKeyword(Keyword keyword)
        {
            if (!_keywords.Contains(keyword))
                _keywords.Add(keyword);
        }
    }
}
