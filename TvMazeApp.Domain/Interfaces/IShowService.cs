using TvMazeApp.Domain.Dtos;
using TvMazeApp.Domain.Entities;

namespace TvMazeApp.Domain.Interfaces;

public interface IShowService
{
    Task<IEnumerable<ShowDto>> GetAllShowsAsync();
    Task FetchAndStoreShowsAsync();
    Task AddShowAsync(ShowDto showDto); 
    Task UpdateShowAsync(int id, ShowDto showDto);
    Task DeleteShowAsync(int id);
}