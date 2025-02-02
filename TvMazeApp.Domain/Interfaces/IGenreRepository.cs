using TvMazeApp.Domain.Entities;

namespace TvMazeApp.Domain.Interfaces;

public interface IGenreRepository
{
    public Task<List<Genre>> GetAll();
}