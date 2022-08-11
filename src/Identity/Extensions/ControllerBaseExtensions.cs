using Identity.Contracts.Responses;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static object TokenResponse(this ControllerBase ctl, string token) => new TokenResponse(token);

        public static object InvalidUsernameOrPassswordResponse(this ControllerBase ctl) => new ErrorResponse("Invalid username or password.");

        public static object ErrorResponse(this ControllerBase ctl, IEnumerable<IdentityError> errors) => new ErrorResponse(errors);

        public static object UpdateUserResponse(this ControllerBase ctl, UserModel user) => new UpdateUserResponse(user.Id, user.Fullname, user.Birthday);

        public static object SignUpResponse(this ControllerBase ctl) => new SignUpResponse("Check your email to confirm your account.");
    }
}