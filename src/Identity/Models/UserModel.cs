using Microsoft.AspNetCore.Identity;

namespace Identity.Models
{
    public class UserModel : IdentityUser<Guid>
    {
        public string? Fullname { get; set; }
        public string? CPF { get; set; }
        public DateTime Birthday { get; set; }
        public ushort Age { get => (ushort) Math.Floor((DateTime.Now - Birthday).TotalDays / 365.25); }
    }
}