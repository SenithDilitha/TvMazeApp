using TvMazeApp.Domain.Dtos;
using TvMazeApp.Domain.Entities;

namespace TvMazeApp.Domain.Interfaces;

public interface IShowService
{
    Task FetchAndStoreShowsAsync();
    Task AddShowAsync(ShowDto showDto); 
    Task UpdateShowAsync(int id, ShowDto showDto);
    Task DeleteShowAsync(int id);
}