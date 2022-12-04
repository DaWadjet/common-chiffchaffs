using Domain.Entities.CaffFileAggregate;
using Domain.Entities.CommentAggregate;
using Domain.Entities.ProductAggregate;
using Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Dal
{
    public class WebshopDbContext : IdentityDbContext<WebshopUser, IdentityRole<Guid>, Guid>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CaffFile> Files { get; set; }


        public WebshopDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CaffFile>().HasOne(x => x.Uploader)
                .WithMany(x => x.OwnFiles)
                .HasForeignKey(x => x.UploaderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<CaffFile>().HasMany(x => x.Customers)
                .WithMany(x => x.BoughtFiles);

            var users = new List<WebshopUser>
            {
                new WebshopUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "test_user",
                    IsAdmin = false,
                },
                new WebshopUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "test_user1",
                    IsAdmin = false,
                },
                new WebshopUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "test_user2",
                    IsAdmin = false,
                },
                new WebshopUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "test_admin",
                    IsAdmin = true,
                }
            };

            PasswordHasher<WebshopUser> passwordHasher = new PasswordHasher<WebshopUser>();
            foreach (var user in users)
            {
                user.PasswordHash = passwordHasher.HashPassword(user, user.UserName);
                user.NormalizedUserName = user.UserName.ToUpper();
                user.SecurityStamp = Guid.NewGuid().ToString();
            }

            builder.Entity<WebshopUser>().HasData(users);

            builder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
