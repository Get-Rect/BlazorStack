using BlazorStack.Data.Models;
using BlazorStack.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorStack.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _users;

        public AccountController(UserManager<ApplicationUser> users)
        {
            _users = users;
        }

        [HttpGet("me", Name = "Get Loggedin User Info")]
        public async Task<IActionResult> GetMe()
        {
            if (HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
            {
                var identity = (ClaimsIdentity)HttpContext.User.Identity;
                var role = identity.FindAll(identity.RoleClaimType)
                    .Select(c =>
                        new RoleClaim
                        {
                            Issuer = c.Issuer,
                            OriginalIssuer = c.OriginalIssuer,
                            Type = c.Type,
                            Value = c.Value,
                            ValueType = c.ValueType
                        }).FirstOrDefault();

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var user = await _users.FindByIdAsync(userId);
                if (user == null) return Unauthorized();

                var userInfo = new UserInfo()
                {
                    Email = user.Email ?? string.Empty,
                    PhotoUrl = user.PhotoUrl ?? string.Empty,
                    RoleClaim = role
                };

                return Ok(userInfo);
            }
            else
            {
                return Unauthorized();
            }
            
        }
    }
}
