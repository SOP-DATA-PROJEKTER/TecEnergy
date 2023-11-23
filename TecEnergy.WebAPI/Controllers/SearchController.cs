using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Services;
using TecEnergy.WebAPI.Services;

namespace TecEnergy.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly SearchService _searchService;

    public SearchController(SearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("{searchInput}")]
    public async Task<ActionResult<IEnumerable<SearchResult>>> GetBySearch(string searchInput)
    {
        if (string.IsNullOrEmpty(searchInput)) return BadRequest("Search request is empty or not filled");
        var result = await _searchService.PerformSearch(searchInput);
        if (result is null) return NotFound("No match found");
        return Ok(result);
    }
}
