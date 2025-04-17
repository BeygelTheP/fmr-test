using System.ComponentModel.DataAnnotations;

namespace FlightPriceAlerts.Core.DTOs
{
    public record CreateUserDto(
        [property: Required][property: EmailAddress] string Email,
        [property: Required] string Name,
        string DeviceToken);
}