using System.ComponentModel.DataAnnotations;

namespace FlightPriceAlerts.Core.DTOs
{
    public record CreateAlertDto(
        [property: Required] string Origin,
        [property: Required] string Destination,
        [property: Required] DateTime DepartDate,
        DateTime? ReturnDate,
        [property: Required] decimal MaxPrice,
        List<string> Airlines,
        string CabinClass);
}