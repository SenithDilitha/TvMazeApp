namespace TvMazeApp.Domain.Interfaces;

public interface IShowService
{
    Task FetchAndStoreShowsAsync();
}