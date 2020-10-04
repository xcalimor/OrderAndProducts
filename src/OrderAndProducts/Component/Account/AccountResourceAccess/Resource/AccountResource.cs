using AccountResourceAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountResourceAccess.Resource
{
    public class AccountResource : IAccountResource
    {
        public Task<User> AddNewUser(User user)
        {
            if(user.Id == Guid.Empty || user.Id == null)
            {
                user.Id = Guid.NewGuid();
            }

            return Task.FromResult(user);
        }

        public IQueryable<User> GetAllUsers()
        {
            return AllUsers().AsQueryable();
        }

        private List<User> AllUsers()
        {
            return new List<User>
            {
                new User
                {
                    Id = Guid.Parse("54660DEF-2977-44EC-A0EC-D599A2AD05BC"),
                    Firstname = "Astrid",
                    Lastname = "Admin",
                    Username = "Astrid Admin",
                    Password = "1234admin",
                    Role = "Admin",
                    RefreshTokens = new List<RefreshToken>{}
                },
                new User
                {
                    Id = Guid.Parse("E90BB633-428F-40DE-AB6E-94B8669833F9"),
                    Firstname = "Urban",
                    Lastname = "User",
                    Username = "Urban User",
                    Password = "1234user",
                    Role = "User",
                    RefreshTokens = new List<RefreshToken>{}
                }
            };
        }
            

        public Task<User> UpdateUser(User user)
        {
            return Task.FromResult(user);
        }
    }
}
