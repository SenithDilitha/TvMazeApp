using Microsoft.AspNetCore.Mvc;
using TvMazeApp.Domain.Interfaces;

namespace TvMazeApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShowsController : ControllerBase
{
    private readonly IShowService _showService;
    
    public ShowsController(IShowService showService)
    {
        _showService = showService;
    }

    [HttpGet("fetch")]
    public async Task<ActionResult> FetchShows()
    {
        await _showService.FetchAndStoreShowsAsync();

        return Ok();
    }
}