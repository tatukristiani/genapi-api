using genapi_api.Data.GenapiData.Entities;
using Microsoft.EntityFrameworkCore;

namespace genapi_api.Data.GenapiData
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<ApiKeyUsage> ApiKeyUsages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();
            });

            // Many-to-many for Users
            modelBuilder.Entity<Organization>()
                .HasMany(o => o.Users)
                .WithMany(u => u.Organizations)
                .UsingEntity<Dictionary<string, object>>(
                    "OrganizationUser",
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Organization>().WithMany().HasForeignKey("OrganizationId").OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("OrganizationId", "UserId");
                        j.ToTable("OrganizationUsers");
                    });

            // Many-to-many for Editors
            modelBuilder.Entity<Organization>()
                .HasMany(o => o.Editors)
                .WithMany(u => u.OrganizationsAsEditor)
                .UsingEntity<Dictionary<string, object>>(
                    "OrganizationEditor",
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Organization>().WithMany().HasForeignKey("OrganizationId").OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("OrganizationId", "UserId");
                        j.ToTable("OrganizationEditors");
                    });

            modelBuilder.Entity<ApiKeyUsage>()
                .HasIndex(u => new { u.ApiKeyId, u.Date })
                .IsUnique(); // Only one usage record per key per day
        }
    }
}
