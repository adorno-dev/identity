using System.Security.Claims;
using Identity.Contracts.Requests;
using Identity.Extensions;
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

        [HttpGet, Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            return await userManager.Users.AsNoTracking().ToListAsync();
        }

        [HttpPut, Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            if (ModelState.IsValid)
            {
                var email = HttpContext.User.FindFirst(w => w.Type.Equals(ClaimTypes.Email));

                var user = await userManager.FindByEmailAsync(email?.Value);

                if (user is null)
                    return NotFound();
                
                user.Fullname = request.Fullname;
                user.Birthday = request.Birthday;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var birthdayClaim = new Claim(ClaimTypes.DateOfBirth, user.Birthday.ToShortDateString());

                    await userManager.AddClaimAsync(user, birthdayClaim);

                    return Ok(this.UpdateUserResponse(user));
                }
                else return BadRequest();
            }

            return BadRequest();
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