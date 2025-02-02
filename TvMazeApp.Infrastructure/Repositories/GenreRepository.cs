using Microsoft.EntityFrameworkCore;
using TvMazeApp.Domain.Entities;
using TvMazeApp.Domain.Interfaces;
using TvMazeApp.Infrastructure.Data;

namespace TvMazeApp.Infrastructure.Repositories;

public class GenreRepository : IGenreRepository
{
    public readonly AppDbContext _context;

    public GenreRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Genre>> GetAll()
    {
        return await _context.Genres.ToListAsync();
    }
}