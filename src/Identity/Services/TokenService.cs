using System.Reflection.Metadata;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Identity.Models;
using Identity.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Services
{
    public class TokenService
    {
        private readonly IOptions<TokenSettings> tokenSettings;
        private readonly UserManager<UserModel> userManager;

        private string GenerateToken(ClaimsIdentity claimsIdentity)
        {
            var handler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor()
            {
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenSettings.Value.GetSecret()),
                SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.UtcNow.AddMinutes(120),
                Subject = claimsIdentity
            };

            var token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }

        public TokenService(IOptions<TokenSettings> tokenSettings, UserManager<UserModel> userManager)
        {
            this.tokenSettings = tokenSettings;
            this.userManager = userManager;
        }

        // public string GenerateToken(UserModel user) => GenerateToken(
        //     new ClaimsIdentity(new Claim[] {
        //         new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  
        //         new Claim(ClaimTypes.Email, user.Email),
        //         new Claim(ClaimTypes.DateOfBirth, user.Birthday.ToShortDateString())
        //     })
        // );

        public string GenerateToken(UserModel user)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.DateOfBirth, user.Birthday.ToShortDateString())
            };

            var userRoles = userManager.GetRolesAsync(user).Result;

            foreach (var role in userRoles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            return GenerateToken(new ClaimsIdentity(claims));
        }

        public string GenerateToken(IEnumerable<Claim> claims) => GenerateToken(new ClaimsIdentity(claims));

        public string GenerateRefreshToken()
        {
            using var generator = RandomNumberGenerator.Create();

            var random = new byte[32];

            generator.GetBytes(random);

            return Convert.ToBase64String(random);
        }

        public ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var parameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(tokenSettings.Value.GetSecret()),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            var claimsPrincipal = handler.ValidateToken(token, parameters, out var securityToken);

            if (securityToken is not JwtSecurityToken securityJwtToken || !
                securityJwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token.");
            
            return claimsPrincipal;
        }

        public string GetUserIdFromRequest(HttpContext context)
        {
            var token = context.Request.Headers.Authorization.First().Split(" ")[1];

            var claims = GetClaimsPrincipalFromExpiredToken(token);

            return claims.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public void GenerateToken(UserModel user, out string token) => token = GenerateToken(user);

        public void GenerateToken(IEnumerable<Claim> claims, out string token) => token = GenerateToken(claims);

        public void GenerateRefreshToken(out string refreshToken) => refreshToken = GenerateRefreshToken();

        public void GetClaimsPrincipalFromExpiredToken(string token, out ClaimsPrincipal claimsPrincipal) 
            => claimsPrincipal = GetClaimsPrincipalFromExpiredToken(token);
    }
}