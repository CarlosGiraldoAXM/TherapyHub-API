using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.SessionNotesStatus;
using TherapuHubAPI.DTOs.Responses.SessionNotesStatus;
using TherapuHubAPI.Models;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SessionNotesStatusController : ControllerBase
{
    private readonly ISessionNotesStatusService _service;
    private readonly ContextDB _context;
    private readonly ILogger<SessionNotesStatusController> _logger;

    public SessionNotesStatusController(
        ISessionNotesStatusService service,
        ContextDB context,
        ILogger<SessionNotesStatusController> logger)
    {
        _service = service;
        _context = context;
        _logger = logger;
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null || !int.TryParse(claim.Value, out var id)) return null;
        return id;
    }

    private async Task<int?> GetActorIdAsync()
    {
        var userId = GetUserId();
        if (userId == null) return null;
        return await _context.Users
            .Where(u => u.Id == userId.Value)
            .Select(u => (int?)u.ActorId)
            .FirstOrDefaultAsync();
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SessionNotesStatusResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SessionNotesStatusResponseDto>>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<SessionNotesStatusResponseDto>>.SuccessResponse(result, "Session note statuses retrieved successfully"));
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SessionNotesStatusResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SessionNotesStatusResponseDto>>>> GetActive()
    {
        var result = await _service.GetActiveAsync();
        return Ok(ApiResponse<IEnumerable<SessionNotesStatusResponseDto>>.SuccessResponse(result, "Active session note statuses retrieved successfully"));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SessionNotesStatusResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SessionNotesStatusResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SessionNotesStatusResponseDto>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<SessionNotesStatusResponseDto>.ErrorResponse("Session note status not found", null, 404));
        return Ok(ApiResponse<SessionNotesStatusResponseDto>.SuccessResponse(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SessionNotesStatusResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<SessionNotesStatusResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<SessionNotesStatusResponseDto>>> Create([FromBody] CreateSessionNotesStatusRequestDto request)
    {
        var actorId = await GetActorIdAsync();
        if (actorId == null)
            return Unauthorized(ApiResponse<SessionNotesStatusResponseDto>.ErrorResponse("Unauthorized", null, 401));

        try
        {
            var created = await _service.CreateAsync(request, actorId.Value);
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                ApiResponse<SessionNotesStatusResponseDto>.SuccessResponse(created, "Session note status created successfully", 201));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating session note status");
            return StatusCode(500, ApiResponse<SessionNotesStatusResponseDto>.ErrorResponse("Internal server error", null, 500));
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SessionNotesStatusResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SessionNotesStatusResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SessionNotesStatusResponseDto>>> Update(int id, [FromBody] UpdateSessionNotesStatusRequestDto request)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, request);
            if (updated == null)
                return NotFound(ApiResponse<SessionNotesStatusResponseDto>.ErrorResponse("Session note status not found", null, 404));
            return Ok(ApiResponse<SessionNotesStatusResponseDto>.SuccessResponse(updated, "Session note status updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating session note status {Id}", id);
            return StatusCode(500, ApiResponse<SessionNotesStatusResponseDto>.ErrorResponse("Internal server error", null, 500));
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var actorId = await GetActorIdAsync();
        if (actorId == null)
            return Unauthorized(ApiResponse<object>.ErrorResponse("Unauthorized", null, 401));

        try
        {
            var deleted = await _service.DeleteAsync(id, actorId.Value);
            if (!deleted)
                return NotFound(ApiResponse<object>.ErrorResponse("Session note status not found", null, 404));
            return Ok(ApiResponse<object>.SuccessResponse(null, "Session note status deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting session note status {Id}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error", null, 500));
        }
    }

    [HttpPatch("{id:int}/toggle-active")]
    [ProducesResponseType(typeof(ApiResponse<SessionNotesStatusResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SessionNotesStatusResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SessionNotesStatusResponseDto>>> ToggleActive(int id)
    {
        try
        {
            var result = await _service.ToggleActiveAsync(id);
            if (result == null)
                return NotFound(ApiResponse<SessionNotesStatusResponseDto>.ErrorResponse("Session note status not found", null, 404));
            return Ok(ApiResponse<SessionNotesStatusResponseDto>.SuccessResponse(result, "Session note status toggled successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling session note status {Id}", id);
            return StatusCode(500, ApiResponse<SessionNotesStatusResponseDto>.ErrorResponse("Internal server error", null, 500));
        }
    }
}
