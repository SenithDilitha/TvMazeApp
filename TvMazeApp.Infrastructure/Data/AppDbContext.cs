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
        modelBuilder.Entity<Show>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<Show>()
            .Property(s => s.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Show>()
            .HasMany(x => x.Genres)
            .WithMany(y => y.Shows);
    }
}