using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<UserModel> userManager;

        public UserController(UserManager<UserModel> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            return await userManager.Users.AsNoTracking().ToListAsync();
        }

        [Route("adult")]
        [HttpGet]
        [Authorize(Policy = "AdultPolicy")]
        public async Task<ActionResult> GetAdultContent()
        {
            await Task.CompletedTask;

            return Ok(new { Message = "You're seeing adult contents. "});
        }
    }
}