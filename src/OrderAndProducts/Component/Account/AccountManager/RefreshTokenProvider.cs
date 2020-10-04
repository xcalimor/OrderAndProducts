using System;

namespace AccountManager
{
    public class RefreshTokenProvider
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
    }
}
