using BulgarianViews.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;



namespace BulgarianViews.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid >
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<LocationPost> LocationPosts { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<FavoriteViews> FavoriteViews { get; set; } = null!;
        public DbSet<Rating> Ratings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Rating>()
             .HasOne(r => r.LocationPost)
             .WithMany(lp => lp.Ratings)
             .HasForeignKey(r => r.LocationPostId)
             .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Tag>()
           .HasData(
                new Tag { Id = Guid.NewGuid(), Name = "Sea" },
                new Tag { Id = Guid.NewGuid(), Name = "Mountain" },
                new Tag { Id = Guid.NewGuid(), Name = "City" },
                new Tag { Id = Guid.NewGuid(), Name = "Village" }

          
           );



        }
    }
}
