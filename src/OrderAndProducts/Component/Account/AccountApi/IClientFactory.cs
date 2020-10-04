using AccountManager.Protos;

namespace AccountApi
{
    public interface IClientFactory
    {
        ManagerService.ManagerServiceClient AccountManagerClient();
    }
}
