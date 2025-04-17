using FlightPriceAlerts.Core.DTOs;
using FlightPriceAlerts.Core.Interfaces;
using FlightPriceAlerts.Core.Models;

namespace FlightPriceAlerts.Core.Services
{
    public class AlertService : IAlertService
    {
        private readonly IAlertRepository _alertRepository;
        private readonly IUserRepository _userRepository;

        public AlertService(IAlertRepository alertRepository, IUserRepository userRepository)
        {
            _alertRepository = alertRepository;
            _userRepository = userRepository;
        }

        public async Task<Alert> GetByIdAsync(Guid id)
        {
            return await _alertRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Alert>> GetByUserIdAsync(Guid userId)
        {
            return await _alertRepository.GetByUserIdAsync(userId);
        }

        public async Task<Alert> CreateAsync(Guid userId, CreateAlertDto alertDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception($"User with ID {userId} not found.");
            }

            var now = DateTime.UtcNow;

            var alert = new Alert
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Origin = alertDto.Origin,
                Destination = alertDto.Destination,
                DepartDate = alertDto.DepartDate,
                ReturnDate = alertDto.ReturnDate,
                MaxPrice = alertDto.MaxPrice,
                Airlines = alertDto.Airlines?.Any() == true ? string.Join(",", alertDto.Airlines) : null,
                CabinClass = alertDto.CabinClass,
                Status = AlertStatus.Active,
                CreatedAt = now,
                UpdatedAt = now
            };

            return await _alertRepository.CreateAsync(alert);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateAlertDto alertDto)
        {
            var existingAlert = await _alertRepository.GetByIdAsync(id);
            if (existingAlert == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(alertDto.Origin))
            {
                existingAlert.Origin = alertDto.Origin;
            }

            if (!string.IsNullOrEmpty(alertDto.Destination))
            {
                existingAlert.Destination = alertDto.Destination;
            }

            if (alertDto.DepartDate.HasValue)
            {
                existingAlert.DepartDate = alertDto.DepartDate.Value;
            }

            existingAlert.ReturnDate = alertDto.ReturnDate;

            if (alertDto.MaxPrice.HasValue)
            {
                existingAlert.MaxPrice = alertDto.MaxPrice.Value;
            }

            if (alertDto.Airlines?.Any() == true)
            {
                existingAlert.Airlines = string.Join(",", alertDto.Airlines);
            }

            if (!string.IsNullOrEmpty(alertDto.CabinClass))
            {
                existingAlert.CabinClass = alertDto.CabinClass;
            }

            if (!string.IsNullOrEmpty(alertDto.Status) && Enum.TryParse<AlertStatus>(alertDto.Status, true, out var status))
            {
                existingAlert.Status = status;
            }

            existingAlert.UpdatedAt = DateTime.UtcNow;

            return await _alertRepository.UpdateAsync(existingAlert);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _alertRepository.DeleteAsync(id);
        }

        public async Task<bool> ToggleStatusAsync(Guid id)
        {
            var alert = await _alertRepository.GetByIdAsync(id);
            if (alert == null)
            {
                return false;
            }

            var newStatus = alert.Status == AlertStatus.Active ? AlertStatus.Paused : AlertStatus.Active;

            return await _alertRepository.UpdateStatusAsync(id, newStatus);
        }
    }
}