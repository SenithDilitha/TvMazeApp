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

    public async Task<Show?> FindByIdNameAndLanguageAsync(int id, string name, string language)
    {
        return await _context.Shows
            .FirstOrDefaultAsync(s => s.Id == id || (s.Name == name && s.Language == language));
    }

    public async Task UpdateAsync(Show show)
    {
        var existingShow = await _context.Shows.FindAsync(show.Id);
        if (existingShow == null)
        {
            throw new KeyNotFoundException($"Show with ID {show.Id} not found.");
        }

        _context.Entry(existingShow).CurrentValues.SetValues(show);
        _context.Shows.Update(existingShow);
    }

    public async Task DeleteAsync(Show show)
    {
        var existingShow = await _context.Shows.FindAsync(show.Id);
        if (existingShow == null)
        {
            throw new KeyNotFoundException($"Show with ID {show.Id} not found.");
        }

        _context.Shows.Remove(existingShow);
    }
}