
using FlightPriceAlerts.Core.DTOs;
using FlightPriceAlerts.Core.Interfaces;
using FlightPriceAlerts.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightPriceAlerts.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly IAlertService _alertService;
        private readonly ILogger<AlertsController> _logger;

        public AlertsController(IAlertService alertService, ILogger<AlertsController> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AlertResponseDto>> GetAlert(Guid id)
        {
            try
            {
                var alert = await _alertService.GetByIdAsync(id);
                
                if (alert == null)
                {
                    return NotFound(new { message = $"Alert with id {id} not found" });
                }
                
                var response = MapAlertToResponseDto(alert);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving alert {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while retrieving the alert" });
            }
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AlertResponseDto>>> GetAlertsByUser(Guid userId)
        {
            try
            {
                var alerts = await _alertService.GetByUserIdAsync(userId);
                var response = alerts.Select(MapAlertToResponseDto).ToList();
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving alerts for user {userId}");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while retrieving alerts" });
            }
        }

        [HttpPost("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AlertResponseDto>> CreateAlert(Guid userId, CreateAlertDto alertDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                var alert = await _alertService.CreateAsync(userId, alertDto);
                
                var response = MapAlertToResponseDto(alert);
                
                return CreatedAtAction(nameof(GetAlert), new { id = alert.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating alert for user {userId}");
                
                if (ex.Message.Contains("not found"))
                {
                    return NotFound(new { message = ex.Message });
                }
                
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while creating the alert" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateAlert(Guid id, UpdateAlertDto alertDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                var success = await _alertService.UpdateAsync(id, alertDto);
                
                if (!success)
                {
                    return NotFound(new { message = $"Alert with id {id} not found" });
                }
                
                return Ok(new { message = "Alert updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating alert {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while updating the alert" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAlert(Guid id)
        {
            try
            {
                var success = await _alertService.DeleteAsync(id);
                
                if (!success)
                {
                    return NotFound(new { message = $"Alert with id {id} not found" });
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting alert {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while deleting the alert" });
            }
        }

        [HttpPost("{id}/toggle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ToggleAlertStatus(Guid id)
        {
            try
            {
                var success = await _alertService.ToggleStatusAsync(id);
                
                if (!success)
                {
                    return NotFound(new { message = $"Alert with id {id} not found" });
                }
                
                return Ok(new { message = "Alert status toggled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling alert status {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while toggling the alert status" });
            }
        }

        private AlertResponseDto MapAlertToResponseDto(Alert alert)
        {
            return new AlertResponseDto
            (
                alert.Id,
                alert.UserId,
                alert.Origin,
                alert.Destination,
                alert.DepartDate,
                alert.ReturnDate,
                alert.MaxPrice,
                string.IsNullOrWhiteSpace(alert.Airlines) 
                    ? new List<string>() 
                    : alert.Airlines.Split(',').ToList(),
                alert.CabinClass,
                alert.Status.ToString(),
                alert.CreatedAt,
                alert.UpdatedAt
            );
        }
    }
}