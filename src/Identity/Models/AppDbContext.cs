using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Models
{
    public class AppDbContext : IdentityDbContext<UserModel, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder mb)
        {
            // mb.Entity<IdentityUserLogin<uint>>()
            //   .HasKey(u => u.UserId);
            
            // mb.Entity<IdentityUserRole<uint>>()
            //   .HasKey(r => r.RoleId);
            
            // mb.Entity<IdentityUserToken<uint>>()
            //   .HasKey(t => t.UserId);

            // mb.Entity<UserModel>()
            //   .HasKey(u => u.Id)
            //   .Metadata
            //   .IsPrimaryKey();
            
            // mb.Entity<UserModel>()
            //   .Property(u => u.Fullname)
            //   .IsRequired();
            
            // mb.Entity<UserModel>()
            //   .Property(u => u.CPF)
            //   .HasMaxLength(11)
            //   .IsRequired();
            
            // mb.Entity<UserModel>()
            //   .Property(u => u.Birthday)
            //   .IsRequired();
            
            // mb.Entity<UserModel>()
            //   .Ignore(u => u.Age);

            mb.Entity<UserModel>()
              .Ignore(u => u.Age);

            base.OnModelCreating(mb);
        }
    }
}