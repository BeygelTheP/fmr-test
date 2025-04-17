namespace FlightPriceAlerts.Core.DTOs
{
    public record AlertResponseDto(
        Guid Id,
        Guid UserId,
        string Origin,
        string Destination,
        DateTime DepartDate,
        DateTime? ReturnDate,
        decimal MaxPrice,
        List<string> Airlines,
        string CabinClass,
        string Status,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}