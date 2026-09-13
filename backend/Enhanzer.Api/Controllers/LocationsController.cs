using Enhanzer.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enhanzer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLocations()
    {
        var locations = await _locationService.GetLocationsAsync();

        return Ok(locations);
    }
}