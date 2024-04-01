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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
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
     

    //[HttpPost("logout", Name = "Logout")]
    //[Authorize]
    //public async Task<IActionResult> Logout()
    //{
    //    try
    //    {
    //        await _signInManager.SignOutAsync();
    //        return Ok();
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest();
    //    }
    //}
}
}
