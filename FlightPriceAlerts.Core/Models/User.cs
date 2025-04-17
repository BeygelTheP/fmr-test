
namespace FlightPriceAlerts.Core.Models
{
    public record User()
    {
        public User(Guid id, string email, string name, string deviceToken, DateTime createdAt, DateTime deletedAt) : this()
        {
            Id = id;
            Email = email;
            Name = name;
            DeviceToken = deviceToken;
            CreatedAt = createdAt;
            UpdatedAt = deletedAt;
        }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string DeviceToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}