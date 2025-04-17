namespace FlightPriceAlerts.Core.DTOs
{
    public record UpdateAlertDto(
        string Origin,
        string Destination,
        DateTime? DepartDate,
        DateTime? ReturnDate,
        decimal? MaxPrice,
        List<string> Airlines,
        string CabinClass,
        string Status);
}