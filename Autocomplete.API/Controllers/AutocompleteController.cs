using Autocomplete.API.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Autocomplete.API.Controllers;

[ApiController]
[Route("api/v1/autocomplete")]
public class AutocompleteController : ControllerBase
{
    private readonly AutoDbContext _db;
    public AutocompleteController(AutoDbContext db) => _db = db;

    [HttpGet("{type}")]
    public async Task<IActionResult> ByType(string type, [FromQuery] string q, [FromQuery] int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(q)) return Ok(Array.Empty<object>());
        var norm = q.ToLowerInvariant();

        var items = await _db.Entries
            .Where(e => e.Type == type && e.NameNorm.StartsWith(norm))
            .OrderBy(e => e.Name)
            .Take(Math.Clamp(limit, 1, 50))
            .Select(e => new { id = e.Id, name = e.Name, extra = e.Extra })
            .ToListAsync();

        return Ok(items);
    }
}
