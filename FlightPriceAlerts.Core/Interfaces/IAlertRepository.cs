using FlightPriceAlerts.Core.Models;

namespace FlightPriceAlerts.Core.Interfaces
{
    public interface IAlertRepository
    {
        Task<Alert> GetByIdAsync(Guid id);
        Task<IEnumerable<Alert>> GetByUserIdAsync(Guid userId);
        Task<Alert> CreateAsync(Alert alert);
        Task<bool> UpdateAsync(Alert alert);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateStatusAsync(Guid id, AlertStatus status);
    }
}