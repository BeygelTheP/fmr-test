
namespace FlightPriceAlerts.Core.DTOs
{
    public record UserResponseDto(
        Guid Id,
        string Email,
        string Name,
        string DeviceToken,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}