using Enhanzer.Api.Data;
using Enhanzer.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Enhanzer.Api.Services;

public class LocationService : ILocationService
{
    private readonly AppDbContext _context;

    public LocationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LocationDto>> GetLocationsAsync()
    {
        return await _context.LocationDetails
            .OrderBy(x => x.LocationName)
            .Select(x => new LocationDto
            {
                LocationCode = x.LocationCode,
                LocationName = x.LocationName
            })
            .ToListAsync();
    }
}