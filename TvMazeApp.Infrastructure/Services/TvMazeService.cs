using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using TvMazeApp.Domain.Dtos;
using TvMazeApp.Domain.Interfaces;

namespace TvMazeApp.Infrastructure.Services;

public class TvMazeService : ITvMazeService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<TvMazeService> _logger;

    public TvMazeService(IConfiguration configuration, ILogger<TvMazeService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = new HttpClient();
    }
    
    public async Task<List<ShowDto>> FetchShowsAsync(int page)
    {
        var url = $"{_configuration["TvMazeBaseUrl"]}/shows?page={page}";
        _logger.LogInformation("Fetching shows from URL: {Url}, pageNumber: {PageNumber}", url, page);

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch shows. Status Code: {StatusCode}", response.StatusCode);
                return new List<ShowDto>();
            }

            var shows = await response.Content.ReadFromJsonAsync<List<ShowDto>>();
            _logger.LogInformation("Successfully fetched {Count} shows from API.", shows?.Count ?? 0);

            return shows ?? new List<ShowDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request error while fetching shows.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error while processing API response.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching shows.");
        }

        return new List<ShowDto>();
    }
}