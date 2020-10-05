using AccountManager.Protos;
using AccountManager.Services;
using AccountResourceAccess.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AccountManager.UnitTest
{
    public class WhenUsingAccountManagerService
    {
        [Fact]
        public async Task AndUserDoesNotExist_NullIsReturnedAsync()
        {
            var loggerMock = Mock.Of<ILogger<AccountManagerService>>();
            var clientFactoryMock = new Mock<IClientFactory>();
            var clientMock = new Moq.Mock<ResourceAccess.ResourceAccessClient>();
            var expectedResponse = new GetUserOnUserNameResponse
            {
                User = null
            };

            var fakeCall = TestCalls.AsyncUnaryCall<GetUserOnUserNameResponse>(Task.FromResult(new GetUserOnUserNameResponse()), Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { });
            clientMock.Setup(m => m.GetUserOnUserNameAsync(Moq.It.IsAny<GetUserOnUserNameRequest>(), null, null, CancellationToken.None)).Returns(fakeCall);
            clientFactoryMock.Setup(c => c.AccountResourceAccessClient()).Returns(clientMock.Object);

            var appConfig = Mock.Of<IOptions<AppSettings>>();
            var service = new AccountManagerService(loggerMock, clientFactoryMock.Object, appConfig);

            AuthenticateResponse authResponse = await service.Authenticate(new AuthenticateRequest { HashedPassword = "pwd", IpAddress = "123", UserName = "user" }, null);

            Assert.True(string.IsNullOrEmpty(authResponse.JwtToken));
            Assert.True(string.IsNullOrEmpty(authResponse.RefreshToken));
            Assert.True(string.IsNullOrEmpty(authResponse.Role));
            Assert.True(string.IsNullOrEmpty(authResponse.UserId));
            Assert.True(string.IsNullOrEmpty(authResponse.UserName));

        }

        [Fact]
        public async Task ToAuthenticateAndUserExist_TokenIsReturnedAsync()
        {
            var loggerMock = Mock.Of<ILogger<AccountManagerService>>();
            var clientFactoryMock = new Mock<IClientFactory>();
            var clientMock = new Moq.Mock<ResourceAccess.ResourceAccessClient>();
            var expectedResponse = new GetUserOnUserNameResponse
            {
                User = new UserMessage
                {
                    Firstname = "Firstname",
                    Lastname = "Lastname",
                    UserId = Guid.NewGuid().ToString(),
                    Role = "User",
                    UserName = "UserUser",
                }
            };

            var fakeCall = TestCalls.AsyncUnaryCall<GetUserOnUserNameResponse>(Task.FromResult(expectedResponse), Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { });
            clientMock.Setup(m => m.GetUserOnUserNameAsync(Moq.It.IsAny<GetUserOnUserNameRequest>(), null, null, CancellationToken.None)).Returns(fakeCall);

            var expectedUpdateResponse = new UpdateUserResponse
            {
                User = new UserMessage
                {
                    Firstname = "Firstname",
                    Lastname = "Lastname",
                    UserId = Guid.NewGuid().ToString(),
                    Role = "User",
                    UserName = "UserUser"
                }
            };
            expectedUpdateResponse.User.RefreshTokens.Add(new RefreshTokenMessage
            {
                CreatedByIp ="123",
                Token = "123123",
            });

            var fakeUIpdateCall = TestCalls.AsyncUnaryCall<UpdateUserResponse>(Task.FromResult(expectedUpdateResponse), Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { });
            clientMock.Setup(m => m.UpdateUserAsync(Moq.It.IsAny<UpdateUserRequest>(), null, null, CancellationToken.None)).Returns(fakeUIpdateCall);
            
            clientFactoryMock.Setup(c => c.AccountResourceAccessClient()).Returns(clientMock.Object);

            var testoption = Options.Create(new AppSettings {SecretString ="secretStuff!ddddddddddddddddddddddddddddddddddddddddddddddddddddddgggggggggggggggggggggggggggggggggggggggggggggggggggggggggggg" });
            var service = new AccountManagerService(loggerMock, clientFactoryMock.Object, testoption);

            AuthenticateResponse authResponse = await service.Authenticate(new AuthenticateRequest { HashedPassword = "pwd", IpAddress = "123", UserName = "user" }, null);

            Assert.False(string.IsNullOrEmpty(authResponse.JwtToken));
            Assert.False(string.IsNullOrEmpty(authResponse.RefreshToken));
            Assert.False(string.IsNullOrEmpty(authResponse.Role));
            Assert.False(string.IsNullOrEmpty(authResponse.UserId));
            Assert.False(string.IsNullOrEmpty(authResponse.UserName));

        }
    }
}
