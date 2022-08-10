using Identity.Contracts.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AuthenticationController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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

                user = new IdentityUser 
                { 
                    UserName = request.Username, 
                    NormalizedUserName = request.Username?.ToUpper().Trim(),
                    Email = request.Email 
                };

                var creation = await userManager.CreateAsync(user, request.Password);

                return creation.Succeeded ?
                    Ok():
                    BadRequest(creation.Errors);
            }

            return BadRequest();
        }

        [Route("signin")]
        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            var validation = false;

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(request.Username);
                
                if (user is not null)
                    validation = await userManager.CheckPasswordAsync(user, request.Password);

                // if (user is not null)
                //     validation = (await signInManager.PasswordSignInAsync(user, request.Password, false, false)).Succeeded;
            }

            return validation ?
                Ok():
                BadRequest(new { Message = "Invalid username or password" });
        }
    }
}