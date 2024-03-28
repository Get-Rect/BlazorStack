using BlazorStack.Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorStack.Services.Models;
using BlazorStack.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BlazorStack.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _users;
        private readonly RoleManager<IdentityRole> _roles;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger, ApplicationDbContext db, UserManager<ApplicationUser> users, RoleManager<IdentityRole> roles)
        {
            _logger = logger;
            _db = db;
            _users = users;
            _roles = roles;
        }

        [HttpGet(Name = "Users")]
        public async Task<List<UserViewModel>> GetUsers(string search = "")
        {
            var users = _db.Users.AsQueryable();
            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            var userViewModels = await users.OrderByDescending(x => x.Email).Select(x => ToUserViewModel(x)).ToListAsync();
            foreach(var user in userViewModels)
            {
                user.Role = roles.FirstOrDefault(x => x.Id == userRoles.FirstOrDefault(y => y.UserId == user.Id)?.RoleId)?.Name ?? string.Empty;
            }
            return userViewModels;
        }

        [HttpPost(Name = "Create User")]
        public async Task<bool> CreateUser(UserViewModel newUser)
        {
            var result = await _users.CreateAsync(new ApplicationUser()
            {
                Email = newUser.Email,
                UserName = newUser.Email,
            }, newUser.Password);

            if (!result.Succeeded) return false;

            var createdUser = await _users.FindByEmailAsync(newUser.Email);
            if (createdUser == null) return false;

            if (string.IsNullOrEmpty(newUser.Role)) return true;

            var roleResult = await _users.AddToRoleAsync(createdUser, newUser.Role);
            if (!roleResult.Succeeded)
            {
                await _users.DeleteAsync(createdUser);
                return false;
            }

            return true;
        }

        [HttpGet("allroles", Name = "Get All Roles")]
        public async Task<List<string>?> GetAllRoles()
        {
            return _roles.Roles.Select(x => x.Name ?? string.Empty)?.Where(x => !string.IsNullOrEmpty(x))?.ToList();
        }

        [HttpGet("{Id}",Name = "User")]
        public async Task<UserDetailsViewModel?> GetUser([FromRoute] string Id)
        {
            var user = await _db.Users.FindAsync(Id);
            var userDetailsViewModel = new UserDetailsViewModel()
            {
                Email = user?.Email ?? string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                PhotoUrl= string.Empty,
            };
            return userDetailsViewModel;
        }

        private static UserViewModel ToUserViewModel(ApplicationUser user)
        {
            return new UserViewModel()
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                PhotoUrl = user.PhotoUrl ?? string.Empty,
            };
        }
    }
}
