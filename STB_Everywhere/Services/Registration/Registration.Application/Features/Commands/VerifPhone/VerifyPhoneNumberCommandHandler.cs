using MediatR;
using RabbitMQ.Client;
using Registration.Application.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registration.Application.Features.Commands.VerifPhone
{
    public class VerifyPhoneNumberCommandHandler : IRequestHandler<VerifyPhoneNumberCommand, Unit>
    {

        private readonly IClientRepository _clientRepository;
        

        public VerifyPhoneNumberCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
            

        }


        public async Task<Unit> Handle(VerifyPhoneNumberCommand request, CancellationToken cancellationToken)
        {
            var client = await _clientRepository.GetByIdAsync(request.ClientId);

            if (client == null)
            {
                throw new Exception($"Client with ID {request.ClientId} not found.");
            }

            if (!client.IsNumberValid)
            {

                client.IsNumberValid = true;

                await _clientRepository.UpdateAsync(client);

               
            }
            return Unit.Value;
        }
    }
}
