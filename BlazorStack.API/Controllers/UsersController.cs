﻿using BlazorStack.Data.Contexts;
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

            if (string.IsNullOrEmpty(newUser.Role)) return Created();

            var roleResult = await _users.AddToRoleAsync(createdUser, newUser.Role);
            if (!roleResult.Succeeded)
            {
                await _users.DeleteAsync(createdUser);
                return BadRequest(roleResult.Errors.Select(x => x.Description));
            }

            return Created();
        }

        [HttpGet("allroles", Name = "Get All Roles")]
        public async Task<List<string>?> GetAllRoles()
        {
            return await _roles.Roles.Select(x => x.Name ?? string.Empty)?.Where(x => !string.IsNullOrEmpty(x))?.ToListAsync();
        }

        [HttpPost("change-password/{id}", Name = "Change Password")]
        public async Task<bool> ChangePassword([FromRoute] string id, [FromBody] ChangePasswordRequest request)
        {
            var newPassword = request.newPassword;
            var user = await _users.FindByIdAsync(id);
            if (user == null) return false;
            var token = await _users.GeneratePasswordResetTokenAsync(user);
            var result = await _users.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        [HttpGet("{Id}",Name = "User")]
        public async Task<UserDetailsViewModel?> GetUser([FromRoute] string Id)
        {
            var user = await _db.Users.FindAsync(Id);
            var roleId = (await _db.UserRoles.FirstOrDefaultAsync(x => x.UserId == Id))?.RoleId ?? string.Empty;
            var roleName = (await _db.Roles.FirstOrDefaultAsync(x => x.Id == roleId))?.Name ?? string.Empty;
            var userDetailsViewModel = new UserDetailsViewModel()
            {
                Email = user?.Email ?? string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                PhotoUrl= string.Empty,
                Role =  roleName,
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
