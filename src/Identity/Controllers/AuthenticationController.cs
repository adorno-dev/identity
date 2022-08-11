using System.Text;
using System.Security.Claims;
using Identity.Contracts.Requests;
using Identity.Extensions;
using Identity.Models;
using Identity.Services;
using Identity.Services.Contracts;
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
        private readonly IEmailService emailService;
        private readonly TokenService tokenService;

        private async Task SendEmailConfirmationAsync(UserModel user)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmation = Url.Action(nameof(EmailConfirmation), "authentication", new { email = user.Email, token }, Request.Scheme);
            
            var message = new StringBuilder();

            message.Append($"<p>Hello, {user.Fullname}.</p>");
            message.Append("<p>");
            message.Append("We received your account request in our system. <br/>");
            message.Append("To complete this process, you should confirm using the link bellow. <br />");
            message.Append("</p>");
            message.Append($"<p><a href='{confirmation}'>Confirm my account</a></p>");
            message.Append("<p>Regards, <br />Support team.</p>");
            
            await emailService.SendEmailAsync(user.Email, "Account confirmation.", string.Empty, message.ToString());
        }

        public AuthenticationController(
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            TokenService tokenService,
            IEmailService emailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.tokenService = tokenService;
            this.emailService = emailService;
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

                    await this.SendEmailConfirmationAsync(user);

                    return Ok(this.SignUpResponse());
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

                var confirmed = await userManager.IsEmailConfirmedAsync(user);

                if (confirmed == false)
                    return Unauthorized(new { message = "Please check your email to confirm this account." });

                tokenService.GenerateToken(user, out string token);

                return Ok(this.TokenResponse(token));
            }

            return Unauthorized(this.InvalidUsernameOrPassswordResponse()); 
        }

        [Route("redefine-password")]
        [HttpPost("{token}")]
        public async Task<IActionResult> RedefinePassword([FromQuery] string token, [FromBody] RedefinePasswordRequest request)
        {
            if (!string.IsNullOrEmpty(token) && ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(request.Email);

                if (user is null)
                    return NotFound();
                
                var result = await userManager.ResetPasswordAsync(user, token, request.Password);

                return result.Succeeded ?
                    Ok():
                    BadRequest();
            }

            return BadRequest();
        }

        [Route("forgot-password")]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(request.Email);

                if (user is null)
                    return NotFound();
                
                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                var confirmation = Url.Action(nameof(RedefinePassword), "authentication", new { token }, Request.Scheme);

                var message = new StringBuilder();

                message.Append($"<p>Hello, {user.Fullname}.</p>");
                message.Append("<p>");
                message.Append("There was a redefinition password request on your account in our website. <br/>");
                message.Append("If it wasn't you, so ignore this message. <br />");
                message.Append("Otherwise you can click bellow to create your new password.");
                message.Append("</p>");
                message.Append($"<p><a href='{confirmation}'>Redefine Password</a></p>");
                message.Append("<p>Regards, <br />Support team.</p>");
                
                await emailService.SendEmailAsync(user.Email, "Password redefinition.", string.Empty, message.ToString());

                return Ok();
            }

            return BadRequest();
        }

        [Route("email-confirmation")]
        [HttpGet]
        public async Task<IActionResult> EmailConfirmation(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return BadRequest();
            
            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
                return NotFound();
            
            var result = await userManager.ConfirmEmailAsync(user, token);

            return result.Succeeded ?
                Ok(new { message = "Your account were confirmed successfully." }):
                BadRequest(new { message = "There was a problem to confirm your account. Please check your data or try again later." });
        }
    }
}