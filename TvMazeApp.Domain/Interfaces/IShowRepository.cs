using TvMazeApp.Domain.Entities;

namespace TvMazeApp.Domain.Interfaces;

public interface IShowRepository
{
    Task<Show?> GetByIdAsync(int showId);
    Task AddAsync(Show show);
    Task<Show?> GetLastShowAsync();
    Task SaveChangesAsync();
}