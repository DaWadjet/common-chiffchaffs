using Domain.Entities.CaffFileAggregate;
using Domain.Entities.CommentAggregate;
using Domain.Entities.ProductAggregate;
using Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public class WebshopDbContext : IdentityDbContext<WebshopUser, IdentityRole<Guid>, Guid>
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CaffFile> Files { get; set; }


    public WebshopDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CaffFile>().HasOne(x => x.Uploader)
            .WithMany(x => x.OwnFiles)
            .HasForeignKey(x => x.UploaderId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CaffFile>().HasMany(x => x.Customers)
            .WithMany(x => x.BoughtFiles);
    }
}

