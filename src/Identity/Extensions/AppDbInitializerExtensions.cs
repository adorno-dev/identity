using Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Extensions
{
    public static class AppDbInitializerExtensions
    {
        public static void UseAppDbInitializer(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var initializer = new AppDbInitializer(
                    scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>(),
                    scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>()
                );

                var execute = async () => {
                    await initializer.InitializeRole();
                    await initializer.InitializeUsers();
                };
                
                execute();
            }
        }
    }
}