using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Domain.Common;

namespace DB.Domain.Entities
{
    public class Institution : AggregateRoot
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Street { get; private set; }
        public string StreetNumber { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }
        public string? Phone { get; private set; }
        public string? Email { get; private set; }
        public string? Website { get; private set; }

        //private Institution() { }

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
    }

}
