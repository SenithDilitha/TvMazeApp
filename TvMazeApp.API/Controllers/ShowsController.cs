using Microsoft.AspNetCore.Mvc;
using TvMazeApp.Domain.Dtos;
using TvMazeApp.Domain.Entities;
using TvMazeApp.Domain.Interfaces;
using TvMazeApp.Infrastructure.Repositories;

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
        try
        {
            await _showService.FetchAndStoreShowsAsync();
            return Ok(new { message = "Shows fetched successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", details = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddShow([FromBody] ShowDto showDto)
    {
        try
        {
            await _showService.AddShowAsync(showDto);
            return CreatedAtAction(nameof(AddShow), new { message = "Show added successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateShow(int id, [FromBody] ShowDto showDto)
    {
        try
        {
            await _showService.UpdateShowAsync(id, showDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteShow(int id)
    {
        try
        {
            await _showService.DeleteShowAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", details = ex.Message });
        }
    }


}