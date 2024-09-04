using Account.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
namespace Account.Infrastructure.Context
{
    public class AccountDbContext
        : IdentityDbContext<
            User, Role, string,
            IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>,
            IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>(b =>
            {
                b.Property(e => e.Initials).HasMaxLength(5);
                b.Property(e => e.LastName)
                .HasMaxLength(50); // Adjusted length as needed
                b.Property(e => e.PhoneNumber)
                .HasMaxLength(20);   // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
            builder.Entity<Role>(b =>
            {
                b.Property(r => r.Description)
                .HasMaxLength(250);
                b.HasMany(e => e.UserRoles)
              .WithOne(e => e.Role)
              .HasForeignKey(ur => ur.RoleId)
              .IsRequired();
            });
            builder.Entity<RolePermission>()
        .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RefreshToken>().HasKey(t => t.Id);

            builder.HasDefaultSchema("Account");

            // Fluent API configurations go here.
            // For example:
            // builder.Entity<SomeEntity>().Property(e => e.PropertyName).IsRequired();
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
    }
}
