using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context;

/// <summary>EF Core database context for the ICA Todo application.</summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    /// <summary>Configures entity mappings, table names, column types, and indexes.</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("TBL_User");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedOnAdd();
            entity.Property(u => u.Username).HasColumnType("varchar(100)").IsRequired();
            entity.Property(u => u.Email).HasColumnType("varchar(255)").IsRequired();
            entity.Property(u => u.PasswordHash).HasColumnType("varchar(255)").IsRequired();
            entity.Property(u => u.CreatedDate).HasColumnType("datetime(6)");
            entity.Property(u => u.UpdatedDate).HasColumnType("datetime(6)");
            entity.Property(u => u.IsActive).HasDefaultValue(true);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("TBL_Task");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).ValueGeneratedOnAdd();
            entity.Property(t => t.Task).HasColumnType("varchar(500)").IsRequired();
            entity.Property(t => t.IsCompleted).HasDefaultValue(false);
            entity.Property(t => t.IsDeleted).HasDefaultValue(false);
            entity.Property(t => t.CreatedDate).HasColumnType("datetime(6)");
            entity.Property(t => t.UpdatedDate).HasColumnType("datetime(6)");
            entity.Property(t => t.CompletedDate).HasColumnType("datetime(6)");
            entity.HasIndex(t => new { t.UserId, t.IsDeleted });
            entity.HasOne(t => t.User)
                  .WithMany(u => u.Tasks)
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
