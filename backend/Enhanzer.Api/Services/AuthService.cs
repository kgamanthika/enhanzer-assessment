using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Enhanzer.Api.Data;
using Enhanzer.Api.DTOs;
using Enhanzer.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Enhanzer.Api.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    private const string ExternalApiUrl =
        "https://ez-staging-api.azurewebsites.net/api/External_Api/POS_Api/Invoke";

    public AuthService(
        HttpClient httpClient,
        AppDbContext context,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var payload = new
        {
            API_Action = "GetLoginData",
            Device_Id = "D001",
            Sync_Time = "",
            Company_Code = request.Email,
            API_Body = new
            {
                Username = request.Email,
                Pw = request.Password
            }
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null
        };

        using var response = await _httpClient.PostAsJsonAsync(
            ExternalApiUrl,
            payload,
            jsonOptions);

        var responseContent =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        List<LocationDto> locations;

        try
        {
            using var document =
                JsonDocument.Parse(responseContent);

            var root = document.RootElement;

            var statusCode = GetIntProperty(
                root,
                "Status_Code");

            if (statusCode != 200)
            {
                throw new UnauthorizedAccessException(
                    "Authentication failed.");
            }

            locations = ExtractLocations(root);
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (JsonException)
        {
            throw new InvalidOperationException(
                "The authentication service returned an invalid response.");
        }

        if (locations.Count == 0)
        {
            throw new UnauthorizedAccessException(
                "Authentication succeeded, but no user locations were returned.");
        }

        await SaveLocationsAsync(locations);

        var token = GenerateJwtToken(request.Email);

        return new LoginResponse
        {
            Token = token,
            Locations = locations
        };
    }

    private static List<LocationDto> ExtractLocations(
        JsonElement root)
    {
        var result = new List<LocationDto>();

        if (!root.TryGetProperty(
                "Response_Body",
                out var responseBody))
        {
            return result;
        }

        if (responseBody.ValueKind != JsonValueKind.Array)
        {
            return result;
        }

        foreach (var userData in responseBody.EnumerateArray())
        {
            if (userData.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            if (!userData.TryGetProperty(
                    "User_Locations",
                    out var userLocations))
            {
                continue;
            }

            if (userLocations.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var location in userLocations.EnumerateArray())
            {
                if (location.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                var locationCode =
                    GetStringProperty(
                        location,
                        "Location_Code");

                var locationName =
                    GetStringProperty(
                        location,
                        "Location_Name");

                if (string.IsNullOrWhiteSpace(locationCode) ||
                    string.IsNullOrWhiteSpace(locationName))
                {
                    continue;
                }

                result.Add(new LocationDto
                {
                    LocationCode = locationCode,
                    LocationName = locationName
                });
            }
        }

        return result
            .GroupBy(x => x.LocationCode)
            .Select(x => x.First())
            .ToList();
    }

    private async Task SaveLocationsAsync(
        List<LocationDto> locations)
    {
        foreach (var location in locations)
        {
            var existing =
                await _context.LocationDetails
                    .FirstOrDefaultAsync(
                        x => x.LocationCode == location.LocationCode);

            if (existing == null)
            {
                _context.LocationDetails.Add(
                    new LocationDetail
                    {
                        LocationCode = location.LocationCode,
                        LocationName = location.LocationName
                    });
            }
            else
            {
                existing.LocationName =
                    location.LocationName;
            }
        }

        await _context.SaveChangesAsync();
    }

    private string GenerateJwtToken(string email)
    {
        var jwtSection =
            _configuration.GetSection("Jwt");

        var key =
            jwtSection["Key"]
            ?? throw new InvalidOperationException(
                "JWT key is not configured.");

        var issuer =
            jwtSection["Issuer"]
            ?? "Enhanzer.Api";

        var audience =
            jwtSection["Audience"]
            ?? "Enhanzer.Angular";

        var expiryMinutes =
            int.TryParse(
                jwtSection["ExpiryMinutes"],
                out var minutes)
                ? minutes
                : 120;

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                email),

            new(
                JwtRegisteredClaimNames.Email,
                email),

            new(
                ClaimTypes.Email,
                email),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires:
                    DateTime.UtcNow.AddMinutes(
                        expiryMinutes),
                signingCredentials:
                    credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    private static string GetStringProperty(
        JsonElement element,
        string propertyName)
    {
        if (!element.TryGetProperty(
                propertyName,
                out var property))
        {
            return string.Empty;
        }

        return property.ValueKind == JsonValueKind.String
            ? property.GetString() ?? string.Empty
            : property.ToString();
    }

    private static int GetIntProperty(
        JsonElement element,
        string propertyName)
    {
        if (!element.TryGetProperty(
                propertyName,
                out var property))
        {
            return 0;
        }

        if (property.ValueKind == JsonValueKind.Number &&
            property.TryGetInt32(out var value))
        {
            return value;
        }

        if (property.ValueKind == JsonValueKind.String &&
            int.TryParse(
                property.GetString(),
                out var stringValue))
        {
            return stringValue;
        }

        return 0;
    }
}