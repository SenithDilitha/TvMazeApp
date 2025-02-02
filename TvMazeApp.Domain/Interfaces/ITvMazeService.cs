using TvMazeApp.Domain.Dtos;

namespace TvMazeApp.Domain.Interfaces;

public interface ITvMazeService
{
    Task<List<ShowDto>> FetchShowsAsync(int page);
}