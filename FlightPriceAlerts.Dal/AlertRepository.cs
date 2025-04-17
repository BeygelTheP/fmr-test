using Microsoft.Data.SqlClient;
using FlightPriceAlerts.Core.Interfaces;
using FlightPriceAlerts.Core.Models;

namespace FlightPriceAlerts.Dal
{
    public class AlertRepository : IAlertRepository
    {
        private readonly DatabaseConnection _connection;

        public AlertRepository(DatabaseConnection connection)
        {
            _connection = connection;
        }

        public async Task<Alert> GetByIdAsync(Guid id)
        {
            using (var connection = _connection.CreateConnection())
            {
                var command = new SqlCommand(@"
                    SELECT Id, UserId, Origin, Destination, DepartDate, ReturnDate, 
                           MaxPrice, Airlines, CabinClass, Status, CreatedAt, UpdatedAt 
                    FROM Alerts 
                    WHERE Id = @Id",
                    connection);

                command.Parameters.Add(new SqlParameter("@Id", id));

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return ReaderToAlert(reader);
                    }

                    return null;
                }
            }
        }

        public async Task<IEnumerable<Alert>> GetByUserIdAsync(Guid userId)
        {
            var alerts = new List<Alert>();

            using (var connection = _connection.CreateConnection())
            {
                var command = new SqlCommand(@"
                    SELECT Id, UserId, Origin, Destination, DepartDate, ReturnDate, 
                           MaxPrice, Airlines, CabinClass, Status, CreatedAt, UpdatedAt 
                    FROM Alerts 
                    WHERE UserId = @UserId",
                    connection);

                command.Parameters.Add(new SqlParameter("@UserId", userId));

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        alerts.Add(ReaderToAlert(reader));
                    }
                }
            }

            return alerts;
        }

        public async Task<Alert> CreateAsync(Alert alert)
        {
            using (var connection = _connection.CreateConnection())
            {
                var command = new SqlCommand(@"
                    INSERT INTO Alerts (Id, UserId, Origin, Destination, DepartDate, ReturnDate, 
                                      MaxPrice, Airlines, CabinClass, Status, CreatedAt, UpdatedAt) 
                    VALUES (@Id, @UserId, @Origin, @Destination, @DepartDate, @ReturnDate, 
                            @MaxPrice, @Airlines, @CabinClass, @Status, @CreatedAt, @UpdatedAt)",
                    connection);

                AddAlertParameters(command, alert);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                return alert;
            }
        }

        public async Task<bool> UpdateAsync(Alert alert)
        {
            using (var connection = _connection.CreateConnection())
            {
                var command = new SqlCommand(@"
                    UPDATE Alerts 
                    SET Origin = @Origin, 
                        Destination = @Destination, 
                        DepartDate = @DepartDate, 
                        ReturnDate = @ReturnDate, 
                        MaxPrice = @MaxPrice, 
                        Airlines = @Airlines, 
                        CabinClass = @CabinClass, 
                        Status = @Status, 
                        UpdatedAt = @UpdatedAt 
                    WHERE Id = @Id",
                    connection);

                AddAlertParameters(command, alert);

                await connection.OpenAsync();
                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using (var connection = _connection.CreateConnection())
            {
                var command = new SqlCommand("DELETE FROM Alerts WHERE Id = @Id", connection);
                command.Parameters.Add(new SqlParameter("@Id", id));

                await connection.OpenAsync();
                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
        }

        public async Task<bool> UpdateStatusAsync(Guid id, AlertStatus status)
        {
            using (var connection = _connection.CreateConnection())
            {
                var command = new SqlCommand(@"
                    UPDATE Alerts 
                    SET Status = @Status, 
                        UpdatedAt = @UpdatedAt 
                    WHERE Id = @Id",
                    connection);

                command.Parameters.Add(new SqlParameter("@Id", id));
                command.Parameters.Add(new SqlParameter("@Status", (int)status));
                command.Parameters.Add(new SqlParameter("@UpdatedAt", DateTime.UtcNow));

                await connection.OpenAsync();
                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
        }

        private Alert ReaderToAlert(SqlDataReader reader)
        {
            var alert = new Alert()
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                Origin = reader.GetString(2),
                Destination = reader.GetString(3),
                DepartDate = reader.GetDateTime(4),
                ReturnDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                MaxPrice = reader.GetDecimal(6),
                Airlines = reader.IsDBNull(7) ? null : reader.GetString(7),
                CabinClass = reader.IsDBNull(8) ? null : reader.GetString(8),
                CreatedAt = reader.GetDateTime(10),
                UpdatedAt = reader.GetDateTime(11),
                Status = (AlertStatus)reader.GetInt32(9)
            };

            return alert;
        }

        private void AddAlertParameters(SqlCommand command, Alert alert)
        {
            command.Parameters.Add(new SqlParameter("@Id", alert.Id));
            command.Parameters.Add(new SqlParameter("@UserId", alert.UserId));
            command.Parameters.Add(new SqlParameter("@Origin", alert.Origin));
            command.Parameters.Add(new SqlParameter("@Destination", alert.Destination));
            command.Parameters.Add(new SqlParameter("@DepartDate", alert.DepartDate));
            command.Parameters.Add(new SqlParameter("@ReturnDate",
                (object)alert.ReturnDate ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@MaxPrice", alert.MaxPrice));
            command.Parameters.Add(new SqlParameter("@Airlines",
                (object)alert.Airlines ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@CabinClass",
                (object)alert.CabinClass ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@Status", (int)alert.Status));
            command.Parameters.Add(new SqlParameter("@CreatedAt", alert.CreatedAt));
            command.Parameters.Add(new SqlParameter("@UpdatedAt", alert.UpdatedAt));
        }
    }
}