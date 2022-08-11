using System.Security.Claims;
using Identity.Contracts.Requests;
using Identity.Extensions;
using Identity.Models;
using Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<UserModel> userManager;
        private readonly SignInManager<UserModel> signInManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly TokenService tokenService;

        public AuthenticationController(
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            TokenService tokenService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.tokenService = tokenService;
        }

        [Route("signup")]
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(request.Email);

                if (user is not null)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return BadRequest(ModelState.Values);
                }

                user = new UserModel 
                { 
                    UserName = request.Username, 
                    NormalizedUserName = request.Username?.ToUpper().Trim(),
                    Email = request.Email,
                    Birthday = request.Birthday,
                    Fullname = request.Fullname,
                    CPF = request.CPF
                };

                var creation = await userManager.CreateAsync(user, request.Password);
                
                if (creation.Succeeded)
                {
                    var birthdayClaim = new Claim(ClaimTypes.DateOfBirth, user.Birthday.ToShortDateString());

                    await userManager.AddClaimAsync(user, birthdayClaim);

                    return Ok();
                }
                else
                {
                    return Unauthorized(this.ErrorResponse(creation.Errors));
                }

            }

            return BadRequest();
        }

        [Route("signin")]
        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is not null && await userManager.CheckPasswordAsync(user, request.Password))
            {
                // await signInManager.PasswordSignInAsync(user, request.Password, false, false)

                tokenService.GenerateToken(user, out string token);

                return Ok(this.TokenResponse(token));
            }

            return Unauthorized(this.InvalidUsernameOrPassswordResponse()); 
        }
    }
}