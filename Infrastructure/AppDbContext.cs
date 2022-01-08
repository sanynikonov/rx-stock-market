using Infrastructure.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : IdentityDbContext<UserModel, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UserModel>()
            .HasMany(u => u.RequestedCompanies)
            .WithOne();

        builder.Entity<CompanyModel>()
            .Property(c => c.Tags)
            .HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries));
        
        var user = new UserModel
        {
            Id = 1,
            UserName = "nickname"
        };

        var ph = new PasswordHasher<UserModel>();
        user.PasswordHash = ph.HashPassword(user, "!Password1");

        builder.Entity<UserModel>().HasData(user);

        base.OnModelCreating(builder);
    }
}