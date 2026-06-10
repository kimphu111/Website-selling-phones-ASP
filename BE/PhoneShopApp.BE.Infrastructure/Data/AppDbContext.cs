using PhoneShopApp.BE.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace PhoneShopApp.BE.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Phone> Phones => Set<Phone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
            entity.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(200).IsRequired();
            entity.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(x => x.Role).HasColumnName("role").HasMaxLength(50).IsRequired();
            entity.HasIndex(x => x.Username).IsUnique();
        });

        modelBuilder.Entity<Phone>(entity =>
        {
            entity.ToTable("phones");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Model).HasColumnName("model").HasMaxLength(200).IsRequired();
            entity.Property(x => x.Brand).HasColumnName("brand").HasMaxLength(200).IsRequired();
            entity.Property(x => x.Specifications).HasColumnName("specifications").HasMaxLength(1000);
            
            // Map Enum Status sang String (VARCHAR trong MySQL)
            entity.Property(x => x.Status)
                  .HasColumnName("status")
                  .HasConversion<string>()
                  .HasMaxLength(50)
                  .IsRequired();
        });
    }
}
