using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MaxMind.GeoIP2;

namespace Common.Logging
{
    public class LoggingDelegatingHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingDelegatingHandler> logger;
        private readonly DatabaseReader geoIpDatabase;

        public LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger, DatabaseReader geoIpDatabase)
        {
            this.logger = logger;
            this.geoIpDatabase = geoIpDatabase;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                string? userIpAddress = request.Headers.Contains("X-Forwarded-For")
                    ? request.Headers.GetValues("X-Forwarded-For").FirstOrDefault()
                    : request.Headers.GetValues("RemoteIpAddress").FirstOrDefault() ?? IPAddress.Loopback.ToString();

                var geoIpResult = geoIpDatabase.City(userIpAddress);
                string userLocation = $"{geoIpResult.Country.Name}, {geoIpResult.City.Name}";

                logger.LogInformation("Sending request to {Url} at {UserLocation}", request.RequestUri, userLocation);

                var response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Received a success response from {Url}", response.RequestMessage.RequestUri);
                }
                else
                {
                    logger.LogWarning("Received a non-success status code {StatusCode} from {Url}",
                        (int)response.StatusCode, response.RequestMessage.RequestUri);
                }

                return response;
            }
            catch (HttpRequestException ex)
                when (ex.InnerException is SocketException se && se.SocketErrorCode == SocketError.ConnectionRefused)
            {
                var hostWithPort = request.RequestUri.IsDefaultPort
                    ? request.RequestUri.DnsSafeHost
                    : $"{request.RequestUri.DnsSafeHost}:{request.RequestUri.Port}";

                logger.LogCritical(ex, "Unable to connect to {Host}. Please check the " +
                                        "configuration to ensure the correct URL for the service " +
                                        "has been configured.", hostWithPort);
            }

            return new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                RequestMessage = request
            };
        }
    }
}
