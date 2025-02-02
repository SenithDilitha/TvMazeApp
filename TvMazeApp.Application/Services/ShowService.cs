using TvMazeApp.Domain.Dtos;
using TvMazeApp.Domain.Entities;
using TvMazeApp.Domain.Interfaces;

namespace TvMazeApp.Application.Services;

public class ShowService : IShowService
{
    private const int BatchSize = 1000;
    private readonly IGenreRepository _genreRepository;
    private readonly IShowRepository _showRepository;
    private readonly ITvMazeService _tvMazeService;

    public ShowService(ITvMazeService tvMazeService, IShowRepository showRepository, IGenreRepository genreRepository)
    {
        _tvMazeService = tvMazeService;
        _showRepository = showRepository;
        _genreRepository = genreRepository;
    }

    public async Task FetchAndStoreShowsAsync()
    {
        var lastShow = await _showRepository.GetLastShowAsync();
        var lastShowId = lastShow?.Id ?? -1;
        var startPageNumber = lastShowId >= 0 ? lastShowId / 250 : 0;
        var page = startPageNumber;

        var allGenres = await _genreRepository.GetAll();
        var existingGenresDict = allGenres.ToDictionary(g => g.Name, g => g);
        var newShows = new List<Show>();
        var showDtos = new List<ShowDto>();

        do
        {
            showDtos = await _tvMazeService.FetchShowsAsync(page);

            foreach (var showDto in showDtos.Where(showDto => showDto.Premiered > new DateTime(2014, 1, 1)))
            {
                var show = new Show
                {
                    Id = showDto.Id,
                    Name = showDto.Name,
                    Language = showDto.Language,
                    Premiered = showDto.Premiered,
                    Summary = showDto.Summary,
                    Genres = showDto.Genres.Select(genreName =>
                    {
                        if (existingGenresDict.TryGetValue(genreName, out var existingGenre)) return existingGenre;

                        var newGenre = new Genre {Name = genreName};
                        existingGenresDict[genreName] = newGenre;
                        return newGenre;
                    }).ToList()
                };

                newShows.Add(show);

                if (newShows.Count >= BatchSize)
                {
                    await SaveNewShowsAsync(newShows);
                    newShows.Clear();
                }
            }

            if (newShows.Count > 0)
            {
                await SaveNewShowsAsync(newShows);
                newShows.Clear();
            }

            page++;
        } while (showDtos.Count > 0);
    }

    private async Task SaveNewShowsAsync(List<Show> newShows)
    {
        foreach (var show in newShows)
            if (await _showRepository.GetByIdAsync(show.Id) == null)
                await _showRepository.AddAsync(show);
        await _showRepository.SaveChangesAsync();
    }
}