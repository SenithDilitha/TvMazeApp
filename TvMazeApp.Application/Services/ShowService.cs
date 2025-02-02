using Microsoft.Extensions.Logging;
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
    private readonly ILogger<ShowService> _logger;

    public ShowService(
        ITvMazeService tvMazeService,
        IShowRepository showRepository,
        IGenreRepository genreRepository,
        ILogger<ShowService> logger)
    {
        _tvMazeService = tvMazeService;
        _showRepository = showRepository;
        _genreRepository = genreRepository;
        _logger = logger;
    }

    public async Task FetchAndStoreShowsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching and storing shows started...");

            var lastShow = await _showRepository.GetLastShowAsync();
            var lastShowId = lastShow?.Id ?? -1;
            var startPageNumber = lastShowId >= 0 ? lastShowId / 250 : 0;
            var page = startPageNumber;

            var allGenres = await _genreRepository.GetAll();
            var existingGenresDict = allGenres.ToDictionary(g => g.Name, g => g);

            var newShows = new List<Show>();
            List<ShowDto> showDtos;

            do
            {
                showDtos = await _tvMazeService.FetchShowsAsync(page);
                if (showDtos.Count == 0) break;

                _logger.LogInformation("Processing {Count} shows from page {Page}", showDtos.Count, page);

                foreach (var showDto in showDtos)
                {
                    if (showDto.Premiered <= new DateTime(2014, 1, 1))
                    {
                        _logger.LogDebug("Skipping show '{ShowName}' with ID {ShowId} due to premiere date {PremiereDate}",
                            showDto.Name, showDto.Id, showDto.Premiered);
                        continue;
                    }

                    var show = MapShowDtoToEntity(showDto, existingGenresDict);
                    newShows.Add(show);

                    if (newShows.Count >= BatchSize)
                    {
                        await SaveNewShowsAsync(newShows);
                        newShows.Clear();
                    }
                }

                page++;
            } while (showDtos.Count > 0);

            if (newShows.Count > 0)
            {
                await SaveNewShowsAsync(newShows);
            }

            _logger.LogInformation("Fetching and storing shows completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching and storing shows.");
            throw;
        }
    }

    public async Task AddShowAsync(ShowDto showDto)
    {
        try
        {
            _logger.LogInformation("Adding a new show: {ShowName} (ID: {ShowId})", showDto.Name, showDto.Id);

            var existingShow = await _showRepository.FindByIdNameAndLanguageAsync(showDto.Id, showDto.Name, showDto.Language);
            if (existingShow != null)
            {
                _logger.LogWarning("Show with ID {ShowId}, Name '{ShowName}', and Language '{Language}' already exists.",
                    showDto.Id, showDto.Name, showDto.Language);
                throw new InvalidOperationException($"Show already exists: ID {showDto.Id}, Name '{showDto.Name}', Language '{showDto.Language}'");
            }

            var allGenres = await _genreRepository.GetAll();
            var existingGenresDict = allGenres.ToDictionary(g => g.Name, g => g);

            var newShow = MapShowDtoToEntity(showDto, existingGenresDict);

            await _showRepository.AddAsync(newShow);
            await _showRepository.SaveChangesAsync();

            _logger.LogInformation("Successfully added show '{ShowName}' (ID: {ShowId})", showDto.Name, showDto.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add show '{ShowName}' (ID: {ShowId})", showDto.Name, showDto.Id);
            throw;
        }
    }

    public async Task UpdateShowAsync(int id, ShowDto showDto)
    {
        try
        {
            _logger.LogInformation("Updating show with ID: {ShowId}", id);

            var existingShow = await _showRepository.GetByIdAsync(id);
            if (existingShow == null)
            {
                _logger.LogWarning("Show with ID {ShowId} not found.", id);
                throw new KeyNotFoundException($"Show with ID {id} not found.");
            }

            existingShow.Name = showDto.Name;
            existingShow.Language = showDto.Language;
            existingShow.Premiered = showDto.Premiered;
            existingShow.Summary = showDto.Summary;

            var allGenres = await _genreRepository.GetAll();
            var existingGenresDict = allGenres.ToDictionary(g => g.Name, g => g);

            existingShow.Genres = showDto.Genres.Select(genreName =>
            {
                if (existingGenresDict.TryGetValue(genreName, out var existingGenre))
                    return existingGenre;

                var newGenre = new Genre { Name = genreName };
                existingGenresDict[genreName] = newGenre;
                return newGenre;
            }).ToList();

            await _showRepository.UpdateAsync(existingShow);
            await _showRepository.SaveChangesAsync();

            _logger.LogInformation("Successfully updated show with ID: {ShowId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating show with ID: {ShowId}", id);
            throw;
        }
    }

    public async Task DeleteShowAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting show with ID: {ShowId}", id);

            var show = await _showRepository.GetByIdAsync(id);
            if (show == null)
            {
                _logger.LogWarning("Show with ID {ShowId} not found.", id);
                throw new KeyNotFoundException($"Show with ID {id} not found.");
            }

            await _showRepository.DeleteAsync(show);
            await _showRepository.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted show with ID: {ShowId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting show with ID: {ShowId}", id);
            throw;
        }
    }

    private Show MapShowDtoToEntity(ShowDto showDto, Dictionary<string, Genre> existingGenresDict)
    {
        return new Show
        {
            Id = showDto.Id,
            Name = showDto.Name,
            Language = showDto.Language,
            Premiered = showDto.Premiered,
            Summary = showDto.Summary,
            Genres = showDto.Genres.Select(genreName =>
            {
                if (existingGenresDict.TryGetValue(genreName, out var existingGenre))
                    return existingGenre;

                var newGenre = new Genre { Name = genreName };
                existingGenresDict[genreName] = newGenre;
                return newGenre;
            }).ToList()
        };
    }

    private async Task SaveNewShowsAsync(List<Show> newShows)
    {
        try
        {
            _logger.LogInformation("Saving {Count} new shows to the database.", newShows.Count);

            foreach (var show in newShows)
            {
                if (await _showRepository.GetByIdAsync(show.Id) == null)
                {
                    await _showRepository.AddAsync(show);
                }
                else
                {
                    _logger.LogWarning("Skipping show with ID {ShowId} as it already exists in the database.", show.Id);
                }
            }

            await _showRepository.SaveChangesAsync();
            _logger.LogInformation("Successfully saved batch of {Count} shows.", newShows.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving new shows.");
            throw;
        }
    }
}
