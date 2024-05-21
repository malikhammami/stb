using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Twilio.TwiML.Messaging;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Serilog;

namespace SMSService.API
{
    public class PhoneNumberVerificationService
    {

        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _twilioPhoneNumber;
        public PhoneNumberVerificationService()
        {
            _accountSid = "ACa2dee29325e4468291abd982c69f24ae";
            _authToken = "44d9af24b3bb75a4f013733f619807c6";
            _twilioPhoneNumber = "+17816536835";

            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "registration_queue_SMS", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartListening(int numOfConsumers)
        {
            for (int i = 0; i < numOfConsumers; i++)
            {
                Task.Factory.StartNew(() => ConsumeMessages());
            }
        }

        private void ConsumeMessages()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = System.Text.Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received message: {message}");
                string[] messages = message.Split(',');

                await Task.Run(() => SendVerificationSMS(messages));

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: "registration_queue_SMS", autoAck: false, consumer: consumer);
            Console.WriteLine("Waiting for messages...");
        }


        private void SendVerificationSMS(string[] messages)
        {
            try
            {
                TwilioClient.Init(_accountSid, _authToken);

                var to = new PhoneNumber(messages[0]);
                var from = new PhoneNumber(_twilioPhoneNumber);

                var smsMessage = MessageResource.Create(
                to: to,
                    from: from,
                    body: "AAAAA"
                );

                Console.WriteLine($"SMS sent successfully. SID: {smsMessage.Sid}");
                Log.Information($"SMS Sent to {to}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending SMS: {ex.Message}");
                Log.Error($"An error occurred when sending SMS : {ex.Message}");
            }
        }

    }
}
