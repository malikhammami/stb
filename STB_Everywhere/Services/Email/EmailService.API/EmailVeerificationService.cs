using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Net.Mail;
using Common.Logging;
using Serilog;

namespace EmailService.API
{
    public class EmailVerificationService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
       
        public EmailVerificationService()
        {
            var factory = new ConnectionFactory() {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "registration_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
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

            _channel.BasicQos(prefetchSize: 0, prefetchCount: 20, global: false);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = System.Text.Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received message: {message}");
                string[] messages = message.Split(',');

                await Task.Run(() => SendVerificationEmail(messages));

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: "registration_queue", autoAck: false, consumer: consumer);
            Console.WriteLine("Waiting for messages...");
        }

        private void SendVerificationEmail(string[] messages)
        {
            try
            {
                var fromAddress = new MailAddress("jerby.146@gmail.com", "Taha Jerbi");
                var toAddress = new MailAddress(messages[0]);
                const string fromPassword = "efvvuvpedcwevaya";
                const string subject = "Email Verification";
                string body = $"Please click the following link to verify your email: http://localhost:28287/api/v1/Register/validateEmail/{messages[1]}";

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                };

                using var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential("jerby.146@gmail.com", fromPassword)
                };

                smtp.Send(message);
                Console.WriteLine("Verification email sent successfully.");
                Log.Information($"Email Sent to {toAddress}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending verification email: {ex.Message}");
                Log.Error($"An error occurred when sending email : {ex.Message}");
            }
        }

        //public void Close()
        //{
        //    _connection.Close();
        //}
    }
}
