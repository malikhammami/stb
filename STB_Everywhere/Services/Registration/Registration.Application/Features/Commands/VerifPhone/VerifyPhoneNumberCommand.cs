using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registration.Application.Features.Commands.VerifPhone
{
    public class VerifyPhoneNumberCommand : IRequest<Unit>
    {
        public Guid ClientId { get; set; }
    }
}
