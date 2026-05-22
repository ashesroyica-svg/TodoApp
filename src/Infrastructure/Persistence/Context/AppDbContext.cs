using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context;

/// <summary>EF Core database context for the ICA Todo application.</summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Project> Projects => Set<Project>();

    /// <summary>Configures entity mappings, table names, column types, and indexes.</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("TBL_User");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedOnAdd();
            entity.Property(u => u.Username).HasColumnType("nvarchar(100)").IsRequired();
            entity.Property(u => u.Email).HasColumnType("nvarchar(255)").IsRequired();
            entity.Property(u => u.PasswordHash).HasColumnType("nvarchar(255)").IsRequired();
            entity.Property(u => u.CreatedDate).HasColumnType("datetime2");
            entity.Property(u => u.UpdatedDate).HasColumnType("datetime2");
            entity.Property(u => u.IsActive).HasDefaultValue(true);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("TBL_Project");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Name).HasColumnType("nvarchar(100)").IsRequired();
            entity.Property(p => p.Description).HasColumnType("nvarchar(500)");
            entity.Property(p => p.Color).HasColumnType("nvarchar(20)").HasDefaultValue("#003087");
            entity.Property(p => p.IsDeleted).HasDefaultValue(false);
            entity.Property(p => p.CreatedDate).HasColumnType("datetime2");
            entity.Property(p => p.UpdatedDate).HasColumnType("datetime2");
            entity.HasIndex(p => new { p.UserId, p.IsDeleted });
            entity.HasOne(p => p.User)
                  .WithMany(u => u.Projects)
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("TBL_Task");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).ValueGeneratedOnAdd();
            entity.Property(t => t.Task).HasColumnType("nvarchar(500)").IsRequired();
            entity.Property(t => t.Description).HasColumnType("nvarchar(2000)");
            entity.Property(t => t.Priority).HasConversion<int>();
            entity.Property(t => t.DueDate).HasColumnType("datetime2");
            entity.Property(t => t.IsCompleted).HasDefaultValue(false);
            entity.Property(t => t.IsDeleted).HasDefaultValue(false);
            entity.Property(t => t.CreatedDate).HasColumnType("datetime2");
            entity.Property(t => t.UpdatedDate).HasColumnType("datetime2");
            entity.Property(t => t.CompletedDate).HasColumnType("datetime2");
            entity.HasIndex(t => new { t.UserId, t.IsDeleted });
            entity.HasOne(t => t.User)
                  .WithMany(u => u.Tasks)
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(t => t.Project)
                  .WithMany(p => p.Tasks)
                  .HasForeignKey(t => t.ProjectId)
                  .OnDelete(DeleteBehavior.ClientSetNull);
        });
    }
}
