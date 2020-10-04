using System;
using System.Collections.Generic;

namespace AccountResourceAccess.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
