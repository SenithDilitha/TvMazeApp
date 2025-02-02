using Microsoft.EntityFrameworkCore;
using TvMazeApp.Domain.Entities;
using TvMazeApp.Domain.Interfaces;
using TvMazeApp.Infrastructure.Data;

namespace TvMazeApp.Infrastructure.Repositories;

public class ShowRepository : IShowRepository
{
    private readonly AppDbContext _context;

    public ShowRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Show?> GetByIdAsync(int showId)
    {
        return await _context.Shows
            .Include(s => s.Genres)
            .FirstOrDefaultAsync(s => s.Id == showId);
    }

    public async Task AddAsync(Show show)
    {
        _context.Shows.Add(show);
    }

    public async Task<Show?> GetLastShowAsync()
    {
        return await _context.Shows.OrderByDescending(s => s!.Id).FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}