using MediatR;
using RabbitMQ.Client;
using Registration.Application.Contracts.Persistence;
using Registration.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registration.Application.Features.Commands
{
    public class ClientRegistrationCommandHandler : IRequestHandler<ClientRegistrationCommand, Unit>
    {

        private readonly IClientRepository _clientRepository;
        private readonly IModel _channel;

        public ClientRegistrationCommandHandler(IClientRepository clientRepository, RabbitMQService rabbitMQService)
        {
            _clientRepository = clientRepository;
            _channel = rabbitMQService.GetChannel();

        }

        public async Task<Unit> Handle(ClientRegistrationCommand request, CancellationToken cancellationToken)
        {
            
            var client = new Client
            {
                
                Civility = request.Civility,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth,
                Address = new Address
                {
                    Country = request.Address.Country,
                    Governance = request.Address.Governance,
                    City = request.Address.City,
                    PostalCode = request.Address.PostalCode,
                    Number = request.Address.Number,
                    Street = request.Address.Street
                }
            };

            await _clientRepository.AddAsync(client);

            // Send a RabbitMQ message for email verification
            var message = $"{client.Email},{client.Id}";
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "registration_queue", basicProperties: null, body: body);
            Console.WriteLine($"Sent message: {message}");

            return Unit.Value;
        }
    }

    public class RabbitMQService : IDisposable
    {
        private readonly IModel _channel;

        public RabbitMQService()
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest"
            };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }

        public IModel GetChannel() => _channel;
    }
}
