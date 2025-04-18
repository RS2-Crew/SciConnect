using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Domain.Common;

namespace DB.Domain.Entities
{
    public class Institution : AggregateRoot
    {
        private readonly List<Instrument> _instruments = new();
        public IReadOnlyCollection<Instrument> Instruments => _instruments.AsReadOnly();

        private readonly List<Microorganism> _microorganisms = new();
        public IReadOnlyCollection<Microorganism> Microorganisms => _microorganisms.AsReadOnly();
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Street { get; private set; }
        public string StreetNumber { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }
        public string? Phone { get; private set; }
        public string? Email { get; private set; }
        public string? Website { get; private set; }



        public Institution(string name, string street, string streetNumber, string city, string country, string? phone, string? email, string? website)
        {
            Name = name;
            Street = street;
            StreetNumber = streetNumber;
            City = city;
            Country = country;
            Phone = phone;
            Email = email;
            Website = website;
        }

        public void AddInstrument(Instrument instrument)
        {
            if (!_instruments.Contains(instrument))
            {
                _instruments.Add(instrument);
            }
        }

        public void AddMicroorgnaism(Microorganism microorganism)
        {
            if (!_microorganisms.Contains(microorganism))
            {
                _microorganisms.Add(microorganism);
            }
        }
    }

}
