using AccountResourceAccess.Protos;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace AccountManager
{
    public class ClientFactory : IClientFactory
    {
        private readonly IConfiguration _configuration;
        private ResourceAccess.ResourceAccessClient _client;

        public ClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public ResourceAccess.ResourceAccessClient AccountResourceAccessClient()
        {
            if (_client == null)
            {
                var channel = GrpcChannel.ForAddress(_configuration["ServiceUrl:AccountResourceAccess"]);
                _client = new ResourceAccess.ResourceAccessClient(channel);
            }

            return _client;
        }
    }
}
