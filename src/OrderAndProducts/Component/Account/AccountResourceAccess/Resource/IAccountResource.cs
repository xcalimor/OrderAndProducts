using AccountResourceAccess.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AccountResourceAccess.Resource
{
    public interface IAccountResource
    {
        IQueryable<User> GetAllUsers();
        Task<User> AddNewUser(User user);
        Task<User> UpdateUser(User user);
    }
}
