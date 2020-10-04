using System.Text.Json.Serialization;

namespace AccountApi.Model
{
    public class AuthenticateResponse
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }

        public AuthenticateResponse(string userId, string firstname, string lastname, string username, string jwtToken, string refreshToken)
        {
            Id = userId;
            Firstname = firstname;
            Lastname = lastname;
            Username = username;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
