using BlazorStack.Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorStack.Services.Models;
using BlazorStack.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorStack.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<WeatherForecastController> _logger;

        public UsersController(ILogger<WeatherForecastController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet(Name = "Users")]
        public async Task<List<UserViewModel>> GetUsers(string search = "")
        {
            var users = _db.Users.AsQueryable();
            var userViewModels = await users.OrderByDescending(x => x.Email).Select(x => ToUserViewModel(x)).ToListAsync();
            return userViewModels;
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
                Email = user.Email ?? string.Empty
            };
        }
    }
}
