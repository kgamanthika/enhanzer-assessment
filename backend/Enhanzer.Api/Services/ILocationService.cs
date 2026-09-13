using Enhanzer.Api.DTOs;

namespace Enhanzer.Api.Services;

public interface ILocationService
{
    Task<List<LocationDto>> GetLocationsAsync();
}