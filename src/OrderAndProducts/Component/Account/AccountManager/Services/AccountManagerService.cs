using AccountManager.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Services
{
    public class AccountManagerService : ManagerService.ManagerServiceBase
    {
        private readonly ILogger<AccountManagerService> _logger;
        private readonly IClientFactory _clientFactory;
        private readonly AppSettings _appSettings;

        public AccountManagerService(ILogger<AccountManagerService> logger, IClientFactory clientFactory, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _appSettings = appSettings.Value;
        }

        public override async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Authenticating " + request.UserName);
            var accessClient = _clientFactory.AccountResourceAccessClient();
            var respone = await accessClient.GetUserOnUserNameAsync(new AccountResourceAccess.Protos.GetUserOnUserNameRequest { UserName = request.UserName, HashedPassword = request.HashedPassword });
            var user = respone.User;
            if (user == null)
                return new AuthenticateResponse();

            var jwtToken = GenerateJwtToken(user.UserId, user.Role, _appSettings.SecretString);
            var newRefreshToken = GenerateRefreshToken(request.IpAddress);

            var updateRequest = new AccountResourceAccess.Protos.UpdateUserRequest
            {
                User = new AccountResourceAccess.Protos.UserMessage
                {
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Role = user.Role,
                    UserId = user.UserId,
                    UserName = user.UserName,
                }
            };

            //Lets move all existing refreshtokens back to the update request so Resource can decide whats new or not.
            foreach(var token in user.RefreshTokens)
            {
                updateRequest.User.RefreshTokens.Add(token);
            }
            
            //Add new token that will be added to Resource
            updateRequest.User.RefreshTokens.Add(new AccountResourceAccess.Protos.RefreshTokenMessage {
                Created = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(newRefreshToken.Created),
                CreatedByIp = newRefreshToken.CreatedByIp,
                Expires = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(newRefreshToken.Expires),
                Token = newRefreshToken.Token
            });
            
            //update Resource and get frsh data back
            var updateResponse = await accessClient.UpdateUserAsync(updateRequest);

            return new AuthenticateResponse
            {
                JwtToken = jwtToken,
                RefreshToken = updateResponse.User.RefreshTokens.OrderByDescending(r=> r.Created).First().Token,
                UserName = user.UserName,
                UserId = user.UserId,
                Role = user.Role
            };
        }

        public override async Task<AuthenticateResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
        {
            var accessClient = _clientFactory.AccountResourceAccessClient();
            var response = await accessClient.GetUserWithRefreshTokenAsync(new AccountResourceAccess.Protos.GetUserWithRefreshTokenRequest {RefreshToken = request.Token });
            if (response.User == null)
                return null;
            var user = response.User;

            var refreshToken = user.RefreshTokens.Single(r => r.Token == request.Token);
            var revokedDate = refreshToken.Revoked.ToDateTime();
            var expireDate = refreshToken.Expires.ToDateTime();

            var isActive = revokedDate == DateTime.MinValue && DateTime.Now < expireDate;
            if (!isActive)
                return null;

            var newRefreshToken = GenerateRefreshToken(request.IpAddress);
            refreshToken.Revoked = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.Now);
            refreshToken.RevokedByIp = request.IpAddress;
            refreshToken.RevokedByToken = newRefreshToken.Token;


            var updateRequest = new AccountResourceAccess.Protos.UpdateUserRequest
            {
                User = new AccountResourceAccess.Protos.UserMessage
                {
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Role = user.Role,
                    UserId = user.UserId,
                    UserName = user.UserName
                }
            };

            //Lets move all existing refreshtokens back to the update request so Resource can decide whats new or not.
            foreach (var token in user.RefreshTokens)
            {
                updateRequest.User.RefreshTokens.Add(token);
            }

            //Add new token that will be added to Resource
            updateRequest.User.RefreshTokens.Add(new AccountResourceAccess.Protos.RefreshTokenMessage
            {
                Created = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(newRefreshToken.Created),
                CreatedByIp = newRefreshToken.CreatedByIp,
                Expires = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(newRefreshToken.Expires),
                Token = newRefreshToken.Token
            });

            //update Resource and get fresh data back
            var updateResponse = await accessClient.UpdateUserAsync(updateRequest);
            var updatedUser = updateResponse.User;
            var newJwtToken = GenerateJwtToken(updatedUser.UserId, updatedUser.Role, _appSettings.SecretString);

            return new AuthenticateResponse
            {
                JwtToken = newJwtToken,
                RefreshToken = updateResponse.User.RefreshTokens.OrderByDescending(r => r.Created).First().Token,
                UserName = user.UserName,
                UserId = user.UserId
            };
        }

        #region "This is pure business logic and should be moved to an engine"

        private string GenerateJwtToken(string userId, string userRole, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Role, userRole),
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshTokenProvider GenerateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new Byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshTokenProvider
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }

        #endregion
    }
}
