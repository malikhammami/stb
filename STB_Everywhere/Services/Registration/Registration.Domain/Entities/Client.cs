using Registration.Domain.Commun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registration.Domain.Entities
{
    public class Client : EntityBase
    {
        public string Civility { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsEmailValid { get; set; }
        public bool IsNumberValid { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public string Country { get; set; }
        public string Governance { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Number { get; set; }
        public string Street { get; set; }
    }
}
