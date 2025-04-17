using FlightPriceAlerts.Core.DTOs;
using FlightPriceAlerts.Core.Interfaces;
using FlightPriceAlerts.Core.Models;

namespace FlightPriceAlerts.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> CreateAsync(CreateUserDto userDto)
        {
            var now = DateTime.UtcNow;

            var user = new User(Guid.NewGuid(), userDto.Email, userDto.Name, userDto.DeviceToken, now, now);

            return await _userRepository.CreateAsync(user);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateUserDto userDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(userDto.Name))
            {
                existingUser.Name = userDto.Name;
            }

            if (!string.IsNullOrEmpty(userDto.DeviceToken))
            {
                existingUser.DeviceToken = userDto.DeviceToken;
            }

            existingUser.UpdatedAt = DateTime.UtcNow;

            return await _userRepository.UpdateAsync(existingUser);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _userRepository.DeleteAsync(id);
        }
    }
}
