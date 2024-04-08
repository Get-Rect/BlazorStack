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

        [HttpGet("roles", Name = "Get Roles")]
        [Authorize]
        public async Task<IActionResult> GetRoles()
        {
            if (HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
            {
                var identity = (ClaimsIdentity)HttpContext.User.Identity;
                var roles = identity.FindAll(identity.RoleClaimType)
                    .Select(c =>
                        new
                        {
                            c.Issuer,
                            c.OriginalIssuer,
                            c.Type,
                            c.Value,
                            c.ValueType
                        });

                return Ok(roles);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("me", Name = "Get Loggedin User Info")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _users.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            return Ok(user);
        }
    }
}
