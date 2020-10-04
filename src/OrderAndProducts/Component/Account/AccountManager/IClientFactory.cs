using AccountResourceAccess.Protos;

namespace AccountManager
{
    public interface IClientFactory
    {
        ResourceAccess.ResourceAccessClient AccountResourceAccessClient();
    }
}
