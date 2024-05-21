using MediatR;
using RabbitMQ.Client;
using Registration.Application.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registration.Application.Features.Commands.VerifEmail
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Unit>  
    {
        private readonly IClientRepository _clientRepository;
        private readonly IModel _channel;

        public VerifyEmailCommandHandler(IClientRepository clientRepository, RabbitMQService rabbitMQService)
        {
            _clientRepository = clientRepository;
            _channel = rabbitMQService.GetChannel();

        }

        public async Task<Unit> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var client = await _clientRepository.GetByIdAsync(request.ClientId);

            if (client == null)
            {
                throw new Exception($"Client with ID {request.ClientId} not found.");
            }

            if (!client.IsEmailValid)
            {
               
                client.IsEmailValid = true;

                await _clientRepository.UpdateAsync(client);

                // Send a RabbitMQ message for Phone Number verification
                var message = $"{client.PhoneNumber},{client.Id}";
                var body = Encoding.UTF8.GetBytes(message);
                _channel.BasicPublish(exchange: "", routingKey: "registration_queue_SMS", basicProperties: null, body: body);
                Console.WriteLine($"Sent message: {message}");
            }
            return Unit.Value;
        }
    }
}
