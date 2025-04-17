namespace FlightPriceAlerts.Core.Models
{
    public record Alert
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal MaxPrice { get; set; }
        public string Airlines { get; set; }
        public string CabinClass { get; set; }
        public AlertStatus Status { get; set; } = AlertStatus.Active;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}