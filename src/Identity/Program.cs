using System.Text;
using Identity.Extensions;
using Identity.Models;
using Identity.Policies;
using Identity.Services;
using Identity.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var tokenSettings = builder.Configuration.GetSection("TokenSettings").Get<TokenSettings>();

builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));

builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<UserModel, IdentityRole<Guid>>(o =>
                {
                    o.User.RequireUniqueEmail = true;
                    o.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-._@+";
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireDigit = false;
                    o.Password.RequiredUniqueChars = 1;
                    o.Password.RequiredLength = 7;
                    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                    o.Lockout.MaxFailedAccessAttempts = 5;
                    o.Lockout.AllowedForNewUsers = true;
                    o.SignIn.RequireConfirmedEmail = false;
                    o.SignIn.RequireConfirmedPhoneNumber = false;
                    o.SignIn.RequireConfirmedAccount = false;                   
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

// builder.Services.AddAuthorization();

// builder.Services.ConfigureApplicationCookie(o =>
// {
//     o.Cookie.Name = "IdentitySample";
//     o.Cookie.HttpOnly = true;
//     o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
//     o.LoginPath = "/users/signin";
//     o.LogoutPath = "/users/signout";
//     o.AccessDeniedPath = "/app/access-denied";
//     o.SlidingExpiration = true;
//     o.ReturnUrlParameter = "returnUrl";
//     o.Events = new CookieAuthenticationEvents()
//     {
//         OnRedirectToAccessDenied = async (ctx) => {
//             ctx.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
//             await Task.CompletedTask;
//         },
//         OnRedirectToLogin = async (ctx) => {
//             ctx.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
//             await Task.CompletedTask;
//         },
//         OnRedirectToReturnUrl = async (ctx) => {
//             ctx.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
//             await Task.CompletedTask;
//         },
//     };
// });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
})
     .AddJwtBearer(tokenOptions =>
     {
        tokenOptions.RequireHttpsMetadata = false;
        tokenOptions.SaveToken = true;
        tokenOptions.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(tokenSettings.GetSecret()),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false
        };
     });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("AdultPolicy", policy => 
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new AdultPolicyRequirement(18));
    });
});

builder.Services.AddSingleton<IAuthorizationHandler, AdultPolicyHandler>();

builder.Services.AddScoped<TokenService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseAppDbInitializer();

app.Run();