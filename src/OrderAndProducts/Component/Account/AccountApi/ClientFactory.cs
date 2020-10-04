using AccountManager.Protos;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace AccountApi
{
    public class ClientFactory : IClientFactory
    {
        private readonly IConfiguration _configuration;
        private ManagerService.ManagerServiceClient _client;

        public ClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public ManagerService.ManagerServiceClient AccountManagerClient()
        {
            if (_client == null)
            {
                var channel = GrpcChannel.ForAddress(_configuration["ServiceUrl:AccountManager"]);
                _client = new ManagerService.ManagerServiceClient(channel);
            }

            return _client;
        }
    }
}
