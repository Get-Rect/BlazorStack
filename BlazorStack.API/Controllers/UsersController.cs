using BlazorStack.Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorStack.Services.Models;
using BlazorStack.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BlazorStack.Services;
using BlazorStack.API.Middleware;

namespace BlazorStack.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _users;
        private readonly RoleManager<IdentityRole> _roles;
        private readonly BlobService _blobs;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger, ApplicationDbContext db, UserManager<ApplicationUser> users, RoleManager<IdentityRole> roles, BlobService blobs)
        {
            _logger = logger;
            _db = db;
            _users = users;
            _roles = roles;
            _blobs = blobs;
        }

        [ServiceFilter(typeof(CacheResourceFilter))]
        [HttpGet(Name = "Users")]
        public async Task<IActionResult> GetUsers(string search = "")
        {
            var users = _db.Users.AsQueryable();
            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            var userViewModels = await users.OrderByDescending(x => x.Email).Select(x => ToUserViewModel(x)).ToListAsync();
            foreach (var user in userViewModels)
            {
                user.Role = roles.FirstOrDefault(x => x.Id == userRoles.FirstOrDefault(y => y.UserId == user.Id)?.RoleId)?.Name ?? string.Empty;
            }
            return Ok(userViewModels);
        }

        [ServiceFilter(typeof(EvictCacheKeysFilter))]
        [HttpPost(Name = "Create User")]
        public async Task<IActionResult> CreateUser(UserViewModel newUser)
        {
            var result = await _users.CreateAsync(new ApplicationUser()
            {
                Email = newUser.Email,
                UserName = newUser.Email,
            }, newUser.Password);

            if (!result.Succeeded) return BadRequest(result.Errors.Select(x => x.Description));

            var createdUser = await _users.FindByEmailAsync(newUser.Email);
            if (createdUser == null) return BadRequest(new List<string>() { "Failed to find created user." });

            if (string.IsNullOrEmpty(newUser.Role)) return Ok(true);

            var roleResult = await _users.AddToRoleAsync(createdUser, newUser.Role);
            if (!roleResult.Succeeded)
            {
                await _users.DeleteAsync(createdUser);
                return BadRequest(roleResult.Errors.Select(x => x.Description));
            }

            return Ok(true);
        }

        [HttpGet("allroles", Name = "Get All Roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                return Ok(await _roles.Roles.Select(x => x.Name ?? string.Empty)?.Where(x => !string.IsNullOrEmpty(x))?.ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string>() { ex.Message });
            }
        }

        [HttpPost("change-password/{id}", Name = "Change Password")]
        public async Task<IActionResult> ChangePassword([FromRoute] string id, [FromBody] ChangePasswordRequest request)
        {
            var newPassword = request.newPassword;
            var user = await _users.FindByIdAsync(id);
            if (user == null) return BadRequest(new List<string>() { "User not found." });
            var token = await _users.GeneratePasswordResetTokenAsync(user);
            var result = await _users.ResetPasswordAsync(user, token, newPassword);
            if (result.Errors.Any()) return BadRequest(result.Errors.Select(x => x.Description).ToList());
            else return Ok(true);
        }

        [ServiceFilter(typeof(EvictCacheKeysFilter))]
        [HttpPost("update-role/{id}", Name = "Update Role")]
        public async Task<IActionResult> UpdateRole([FromRoute] string id, [FromBody] UpdateRoleRequest request)
        {
            var user = await _users.FindByIdAsync(id);
            if (user == null) return BadRequest(new List<string>() { "User not found." });
            var userRoles = await _users.GetRolesAsync(user);
            if (userRoles.Any())
            {
                var removeRolesResult = await _users.RemoveFromRolesAsync(user, userRoles);
                if (!removeRolesResult.Succeeded) return BadRequest(removeRolesResult.Errors.Select(x => x.Description).ToList());
            }

            var newRoleName = request.NewRole;
            if (string.IsNullOrEmpty(newRoleName)) return Ok(true);

            var newRole = _roles.RoleExistsAsync(newRoleName);
            if (newRole is null) return BadRequest("Role does not exist.");

            var result = await _users.AddToRoleAsync(user, newRoleName);
            if (!result.Succeeded) return BadRequest(result.Errors.Select(x => x.Description).ToList());
            else return Ok(true);
        }

        [ServiceFilter(typeof(EvictCacheKeysFilter))]
        [HttpDelete("{id}", Name = "Delete User")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            var user = await _users.FindByIdAsync(id);
            if (user is null) return BadRequest(new List<string>() { "User not found." });
            var result = await _users.DeleteAsync(user);
            if (result.Errors.Any()) return BadRequest(result.Errors.Select(x => x.Description).ToList());
            else return Ok(true);
        }

        [ServiceFilter(typeof(CacheResourceFilter))]
        [HttpGet("{Id}", Name = "User")]
        public async Task<IActionResult> GetUser([FromRoute] string Id)
        {
            var user = await _db.Users.FindAsync(Id);
            if (user is null) return BadRequest(new List<string>() { "Failed to find user." });

            var roleId = (await _db.UserRoles.FirstOrDefaultAsync(x => x.UserId == Id))?.RoleId ?? string.Empty;
            var roleName = (await _db.Roles.FirstOrDefaultAsync(x => x.Id == roleId))?.Name ?? string.Empty;
            var userDetailsViewModel = new UserDetailsViewModel()
            {
                Email = user?.Email ?? string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                PhotoUrl = user?.PhotoUrl ?? string.Empty,
                Role = roleName,
            };
            return Ok(userDetailsViewModel);
        }

        [ServiceFilter(typeof(EvictCacheKeysFilter))]
        [HttpPost("{userId}/upload-profile-photo")]
        public async Task<IActionResult> UploadProfilePhoto([FromRoute] string userId, [FromBody] UploadRequest request)
        {
            var user = await _users.FindByIdAsync(userId);
            if (user is null) return BadRequest(new List<string>() { "User not found." });

            if (string.IsNullOrEmpty(request.Base64)) return BadRequest(new List<string>() { "Profile photo was empty or missing." });

            var bytes = Convert.FromBase64String(request.Base64);
            using (var memoryStream = new MemoryStream(bytes))
            {
                var uri = await _blobs.UploadProfilePhoto(userId, memoryStream);
                if (string.IsNullOrEmpty(uri)) return BadRequest(new List<string>() { "Failed to upload profile photo." });
                user.PhotoUrl = uri;
                var updateUserResponse = await _users.UpdateAsync(user);
                if (!updateUserResponse.Succeeded) return BadRequest(updateUserResponse.Errors.Select(x => x.Description));
                //TODO: Update logic to delete photo if updating user fails.
                return Ok(new PhotoUploadResponse() { Uri = uri });
            }
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
