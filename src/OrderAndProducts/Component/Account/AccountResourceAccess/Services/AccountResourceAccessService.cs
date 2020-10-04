using AccountResourceAccess.Entity;
using AccountResourceAccess.Protos;
using AccountResourceAccess.Resource;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AccountResourceAccess.Services
{
    public class AccountResourceAccessService : ResourceAccess.ResourceAccessBase
    {
        private readonly ILogger<AccountResourceAccessService> _logger;
        private readonly IAccountResource _accountResource;
        public AccountResourceAccessService(ILogger<AccountResourceAccessService> logger, IAccountResource accountResource)
        {
            _logger = logger;
            _accountResource = accountResource;
        }

        public override async Task<GetUserOnUserNameResponse> GetUserOnUserName(GetUserOnUserNameRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Getting user on username  " + request.UserName);

            var currentUser = await Task.Run(() =>
            {
               return _accountResource.GetAllUsers().SingleOrDefault(u => u.Username == request.UserName);
            });

            if (currentUser != null)
            {
                if(sha256_hash(currentUser.Password) == sha256_hash(request.HashedPassword))
                {
                    var result = new GetUserOnUserNameResponse
                    {
                        User = new UserMessage
                        {
                            UserId = currentUser.Id.ToString(),
                            Firstname = currentUser.Firstname,
                            Lastname = currentUser.Lastname,
                            Role = currentUser.Role,
                            UserName = currentUser.Username,
                        }
                    };
                    foreach(var token in currentUser.RefreshTokens)
                    {
                        result.User.RefreshTokens.Add(new RefreshTokenMessage {
                            Created = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(token.Created), 
                            CreatedByIp = token.CreatedByIp, 
                            Expires = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(token.Expires), 
                            Token = token.Token,
                            Revoked = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(token.Revoked??DateTime.MinValue),
                            RevokedByIp = token.RevokedByIp,
                            RevokedByToken = token.RevokedByIp
                        });
                    }
                    return result;
                }
            }

            return new GetUserOnUserNameResponse(); ;
        }

        public override async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            var id = Guid.Parse(request.User.UserId);
            var userToUpdate =  _accountResource.GetAllUsers().Single(u => u.Id == id);
            foreach(var token in request.User.RefreshTokens)
            {
                if(!userToUpdate.RefreshTokens.Any(r=> r.Token == token.Token))
                {
                    userToUpdate.RefreshTokens.Add(new RefreshToken {
                        Created = token.Created.ToDateTime(),
                        CreatedByIp = token.CreatedByIp,
                        Expires = token.Expires.ToDateTime(),
                        Token = token.Token,
                        Revoked = token.Revoked == null ? DateTime.MinValue : token.Revoked.ToDateTime(),
                        RevokedByIp = token.RevokedByIp,
                        RevokedByToken = token.RevokedByIp
                    });
                }
            }

            var updated = await _accountResource.UpdateUser(userToUpdate);
            var response =  new UpdateUserResponse() { User = new UserMessage {
                UserId = updated.Id.ToString(),
                Firstname = updated.Firstname,
                Lastname = updated.Lastname,
                Role = updated.Role,
                UserName = updated.Username,
            } };
            foreach (var token in updated.RefreshTokens)
            {
                DateTime revokeDate = token.Revoked == null ? DateTime.MinValue.ToUniversalTime() : (DateTime)token.Revoked;
                response.User.RefreshTokens.Add(new RefreshTokenMessage
                {
                    Created = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(token.Created.ToUniversalTime()),
                    CreatedByIp = token.CreatedByIp,
                    Expires = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(token.Expires.ToUniversalTime()),
                    Token = token.Token,
                    Revoked = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(revokeDate.ToUniversalTime()),
                    RevokedByIp = token.RevokedByIp,
                    RevokedByToken = token.RevokedByIp
                });
            }
            return response;
        }

        //public override Task<GetUserWithRefreshTokenResponse> GetUserWithRefreshToken(GetUserWithRefreshTokenRequest request, ServerCallContext context)
        //{
        //    var id = Guid.Parse(request.User.UserId);
        //    var userToUpdate = _accountResource.GetAllUsers()
        //}

        private static String sha256_hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
