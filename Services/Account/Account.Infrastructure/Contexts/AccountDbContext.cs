using Account.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Account.Infrastructure.Context
{
    public partial class AccountDbContext(DbContextOptions<AccountDbContext> options) : IdentityDbContext<User, Role,string>(options)
    {
        public AccountDbContext() : this(new DbContextOptions<AccountDbContext>()) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().Property(e => e.Initials).HasMaxLength(5);
            builder.Entity<User>()
                .Property(e => e.LastName)
                .HasMaxLength(50); // Adjusted length as needed

            builder.Entity<User>()
                .Property(e => e.PhoneNumber)
                .HasMaxLength(20); // Adjusted length as needed

            // Configure Role entity
            builder.Entity<Role>()
                .Property(r => r.Description)
                .HasMaxLength(250);

            builder.Entity<RefreshToken>().HasKey(t => t.Id);


            builder.HasDefaultSchema("Account");
            // Fluent API configurations go here.
            // For example:
            // builder.Entity<SomeEntity>().Property(e => e.PropertyName).IsRequired();
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Permission> Permissions { get; set; }

    }
}
