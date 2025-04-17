using FlightPriceAlerts.Core.DTOs;
using FlightPriceAlerts.Core.Models;

namespace FlightPriceAlerts.Core.Interfaces
{
    public interface IAlertService
    {
        Task<Alert> GetByIdAsync(Guid id);
        Task<IEnumerable<Alert>> GetByUserIdAsync(Guid userId);
        Task<Alert> CreateAsync(Guid userId, CreateAlertDto alertDto);
        Task<bool> UpdateAsync(Guid id, UpdateAlertDto alertDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ToggleStatusAsync(Guid id);
    }
}

