using Microsoft.EntityFrameworkCore;
using TvMazeApp.Domain.Entities;

namespace TvMazeApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    public DbSet<Show> Shows { get; set; }
    public DbSet<Genre> Genres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).ValueGeneratedNever();
            entity.HasMany(s => s.Genres).WithMany(g => g.Shows);
            entity.HasIndex(s => new { s.Name, s.Language });
            entity.HasIndex(s => s.Name);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasIndex(g => g.Name).IsUnique();
        });
    }
}