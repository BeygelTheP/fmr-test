using Microsoft.Data.SqlClient;
using FlightPriceAlerts.Core.Interfaces;
using FlightPriceAlerts.Core.Models;

namespace FlightPriceAlerts.Dal
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseConnection _connection;

        public UserRepository(DatabaseConnection connection)
        {
            _connection = connection;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            using (var connection = _connection.CreateConnection())
            {
                var command = new SqlCommand(
                    "SELECT Id, Email, Name, DeviceToken, CreatedAt, UpdatedAt FROM Users WHERE Id = @Id",
                    connection);

                command.Parameters.Add(new SqlParameter("@Id", id));

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return ReaderToUser(reader);
                    }

                    return null;
                }
            }
        }

        private static User ReaderToUser(SqlDataReader reader)
        {
            return new User
            (
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.GetDateTime(4),
                reader.GetDateTime(5)
            );
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = new List<User>();

            using (var connection = _connection.CreateConnection())
            {
                var command = new SqlCommand(
                    "SELECT Id, Email, Name, DeviceToken, CreatedAt, UpdatedAt FROM Users",
                    connection);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(ReaderToUser(reader));
                    }
                }
            }

            return users;
        }

        public async Task<User> CreateAsync(User user)
        {
            using (var connection = _connection.CreateConnection())
            {
                var command = new SqlCommand(@"
                    INSERT INTO Users (Id, Email, Name, DeviceToken, CreatedAt, UpdatedAt) 
                    VALUES (@Id, @Email, @Name, @DeviceToken, @CreatedAt, @UpdatedAt)",
                    connection);

                command.Parameters.Add(new SqlParameter("@Id", user.Id));
                command.Parameters.Add(new SqlParameter("@Email", user.Email));
                command.Parameters.Add(new SqlParameter("@Name", user.Name));
                command.Parameters.Add(new SqlParameter("@DeviceToken",
                    (object)user.DeviceToken ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@CreatedAt", user.CreatedAt));
                command.Parameters.Add(new SqlParameter("@UpdatedAt", user.UpdatedAt));

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                return user;
            }
        }

        public async Task<bool> UpdateAsync(User user)
        {
            using (var connection = _connection.CreateConnection())
            {
                var command = new SqlCommand(@"
                    UPDATE Users 
                    SET Name = @Name, 
                        DeviceToken = @DeviceToken, 
                        UpdatedAt = @UpdatedAt 
                    WHERE Id = @Id",
                    connection);

                command.Parameters.Add(new SqlParameter("@Id", user.Id));
                command.Parameters.Add(new SqlParameter("@Name", user.Name));
                command.Parameters.Add(new SqlParameter("@DeviceToken",
                    (object)user.DeviceToken ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@UpdatedAt", user.UpdatedAt));

                await connection.OpenAsync();
                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using (var connection = _connection.CreateConnection())
            {
                var command = new SqlCommand("DELETE FROM Users WHERE Id = @Id", connection);
                command.Parameters.Add(new SqlParameter("@Id", id));

                await connection.OpenAsync();
                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
        }
    }
}