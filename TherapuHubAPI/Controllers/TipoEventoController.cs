using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.Events;
using TherapuHubAPI.DTOs.Responses.Events;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TipoEventoController : ControllerBase
{
    private readonly ITipoEventoService _tipoEventoService;
    private readonly ILogger<TipoEventoController> _logger;

    public TipoEventoController(ITipoEventoService tipoEventoService, ILogger<TipoEventoController> logger)
    {
        _tipoEventoService = tipoEventoService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all active event types (used by calendar/selectors)
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TipoEventoResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<TipoEventoResponseDto>>>> GetTiposEvento()
    {
        var tipos = await _tipoEventoService.GetTiposEventoActivosAsync();
        return Ok(ApiResponse<IEnumerable<TipoEventoResponseDto>>.SuccessResponse(tipos, "Event types retrieved successfully", 200));
    }

    /// <summary>
    /// Gets all event types including inactive (admin view)
    /// </summary>
    [HttpGet("all")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TipoEventoResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<TipoEventoResponseDto>>>> GetAll()
    {
        var tipos = await _tipoEventoService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<TipoEventoResponseDto>>.SuccessResponse(tipos, "Event types retrieved successfully", 200));
    }

    /// <summary>
    /// Gets an event type by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<TipoEventoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TipoEventoResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TipoEventoResponseDto>>> GetById(int id)
    {
        var tipo = await _tipoEventoService.GetByIdAsync(id);
        if (tipo == null)
            return NotFound(ApiResponse<TipoEventoResponseDto>.ErrorResponse(
                "Event type not found",
                new List<string> { $"No event type found with ID {id}" },
                404));

        return Ok(ApiResponse<TipoEventoResponseDto>.SuccessResponse(tipo, "Event type retrieved successfully", 200));
    }

    /// <summary>
    /// Creates a new event type
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<TipoEventoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<TipoEventoResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<TipoEventoResponseDto>>> Create([FromBody] CreateEventTypeRequestDto request)
    {
        try
        {
            var created = await _tipoEventoService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.IdTipoEvento },
                ApiResponse<TipoEventoResponseDto>.SuccessResponse(created, "Event type created successfully", 201));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event type");
            return StatusCode(500, ApiResponse<TipoEventoResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while creating the event type" },
                500));
        }
    }

    /// <summary>
    /// Updates an existing event type
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<TipoEventoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TipoEventoResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TipoEventoResponseDto>>> Update(int id, [FromBody] UpdateEventTypeRequestDto request)
    {
        try
        {
            var updated = await _tipoEventoService.UpdateAsync(id, request);
            if (updated == null)
                return NotFound(ApiResponse<TipoEventoResponseDto>.ErrorResponse(
                    "Event type not found",
                    new List<string> { $"No event type found with ID {id}" },
                    404));

            return Ok(ApiResponse<TipoEventoResponseDto>.SuccessResponse(updated, "Event type updated successfully", 200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event type {Id}", id);
            return StatusCode(500, ApiResponse<TipoEventoResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while updating the event type" },
                500));
        }
    }

    /// <summary>
    /// Deletes an event type by ID
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var deleted = await _tipoEventoService.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.ErrorResponse(
                    "Event type not found",
                    new List<string> { $"No event type found with ID {id}" },
                    404));

            return Ok(ApiResponse<object>.SuccessResponse(null, "Event type deleted successfully", 200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event type {Id}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                "Internal server error",
                new List<string> { "Could not delete the event type. It may be in use by existing events." },
                500));
        }
    }

    /// <summary>
    /// Toggles the active status of an event type
    /// </summary>
    [HttpPatch("{id:int}/toggle-active")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<TipoEventoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TipoEventoResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TipoEventoResponseDto>>> ToggleActive(int id)
    {
        try
        {
            var result = await _tipoEventoService.ToggleActiveAsync(id);
            if (result == null)
                return NotFound(ApiResponse<TipoEventoResponseDto>.ErrorResponse(
                    "Event type not found",
                    new List<string> { $"No event type found with ID {id}" },
                    404));

            return Ok(ApiResponse<TipoEventoResponseDto>.SuccessResponse(result, "Event type status updated successfully", 200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling active status for event type {Id}", id);
            return StatusCode(500, ApiResponse<TipoEventoResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while updating the event type status" },
                500));
        }
    }
}
