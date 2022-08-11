using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Policies
{
    public class AdultPolicyRequirement : IAuthorizationRequirement
    {
        public ushort Age { get; set; }

        public AdultPolicyRequirement(ushort age) => this.Age = age;
    }

    public class AdultPolicyHandler : AuthorizationHandler<AdultPolicyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdultPolicyRequirement requirement)
        {
            var claim = context.User.FindFirst(c => c.Type.Equals(ClaimTypes.DateOfBirth));

            if (claim is null)
                return Task.CompletedTask;

            var age = (DateTime.Now - DateTime.Parse(claim.Value)).Days / 365.25;

            if (age >= requirement.Age)
                context.Succeed(requirement);
            else
                context.Fail();
            
            return Task.CompletedTask;
        }
    }
}