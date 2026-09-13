namespace Enhanzer.Api.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;

    public List<LocationDto> Locations { get; set; } = new();
}