using TvMazeApp.Domain.Entities;

namespace TvMazeApp.Domain.Interfaces;

public interface IShowRepository
{
    Task<Show?> GetByIdAsync(int showId);
    Task AddAsync(Show show);
    Task<Show?> GetLastShowAsync();
    Task SaveChangesAsync();
    Task<Show?> FindByIdNameAndLanguageAsync(int id, string name, string language);
    Task UpdateAsync(Show show);
    Task DeleteAsync(Show show);
}