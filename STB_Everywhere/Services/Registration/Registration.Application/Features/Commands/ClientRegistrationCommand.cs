using MediatR;
using Registration.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registration.Application.Features.Commands
{
    public class ClientRegistrationCommand : IRequest<Unit>
    {
        public string Civility { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string IsEmailValid { get; set; }
        public string IsNumberValid  { get; set; } 
        public DateTime DateOfBirth { get; set; }
        public Address Address { get; set; }
    }
}
