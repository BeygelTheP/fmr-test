using FlightPriceAlerts.Core.DTOs;
using FlightPriceAlerts.Core.Models;

namespace FlightPriceAlerts.Core.Interfaces
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(Guid id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(CreateUserDto userDto);
        Task<bool> UpdateAsync(Guid id, UpdateUserDto userDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
