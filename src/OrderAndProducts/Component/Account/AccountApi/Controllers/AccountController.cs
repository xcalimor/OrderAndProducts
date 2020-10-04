using System;
using System.Threading.Tasks;
using AccountApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AccountApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IClientFactory _clientFactory;

        public AccountController(ILogger<AccountController> logger, IClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateRequest request)
        {
            var client = _clientFactory.AccountManagerClient();
            var ip = ipAddress();
            var response = await client.AuthenticateAsync(new AccountManager.Protos.AuthenticateRequest {HashedPassword = request.Password, UserName = request.Username, IpAddress = ip });
            if (string.IsNullOrEmpty(response.UserId))
                return BadRequest(new { message = "Username or password is incorrect" });

            SetTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var client = _clientFactory.AccountManagerClient();

            var refreshToken = Request.Cookies["refreshToken"];
            var ip = ipAddress();
            var response = await client.RefreshTokenAsync(new AccountManager.Protos.RefreshTokenRequest {IpAddress = ip, Token = refreshToken });

            if (response == null)
                return Unauthorized(new { message = "Invalid Token" });

            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/refresh-tokens")]
        public async Task<IActionResult> GetRefreshToken(Guid id)
        {
            return Ok();
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                Path = "/",
                HttpOnly = false,
                IsEssential = true, //<- there
                Expires = DateTime.Now.AddMonths(1)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            var headerKey = "X-Forwarded-For";

            if (Request.Headers.ContainsKey(headerKey))
                return Request.Headers[headerKey];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
