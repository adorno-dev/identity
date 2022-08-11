
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Identity.Models
{
    public class AppDbInitializer
    {
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly UserManager<UserModel> userManager;

        public AppDbInitializer(RoleManager<IdentityRole<Guid>> roleManager, UserManager<UserModel> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public async Task InitializeRole()
        {
            if (await roleManager.RoleExistsAsync("admin"))
                return;
            
            var role = new IdentityRole<Guid>();

            role.Name = "admin";

            await roleManager.CreateAsync(role);
        }

        public async Task InitializeUsers()
        {
            if (await userManager.FindByEmailAsync("admin@identity.com") is not null)
                return;
            
            var user = new UserModel();

            user.Fullname = "Administrator";
            user.UserName = "admin";
            user.Email = "admin@identity.com";
            user.Birthday = new DateTime(1980, 01, 01);
            user.PhoneNumber = "00000000000";
            user.CPF = "00000000000";
            user.EmailConfirmed = true;

            var creation = await userManager.CreateAsync(user, "Admin@2022");

            if (creation.Succeeded)
            {
                var birthdayClaim = new Claim(ClaimTypes.DateOfBirth, user.Birthday.ToShortDateString());
                
                await userManager.AddToRoleAsync(user, "admin");

                await userManager.AddClaimAsync(user, birthdayClaim);
            }
        }
    }
}