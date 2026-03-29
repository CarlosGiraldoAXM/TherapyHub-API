using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.GoalTrackerStatus;
using TherapuHubAPI.DTOs.Responses.GoalTrackerStatus;
using TherapuHubAPI.Models;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GoalTrackerStatusController : ControllerBase
{
    private readonly IGoalTrackerStatusService _service;
    private readonly ContextDB _context;
    private readonly ILogger<GoalTrackerStatusController> _logger;

    public GoalTrackerStatusController(
        IGoalTrackerStatusService service,
        ContextDB context,
        ILogger<GoalTrackerStatusController> logger)
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
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<GoalTrackerStatusResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<GoalTrackerStatusResponseDto>>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<GoalTrackerStatusResponseDto>>.SuccessResponse(result, "Goal tracker statuses retrieved successfully"));
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<GoalTrackerStatusResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<GoalTrackerStatusResponseDto>>>> GetActive()
    {
        var result = await _service.GetActiveAsync();
        return Ok(ApiResponse<IEnumerable<GoalTrackerStatusResponseDto>>.SuccessResponse(result, "Active goal tracker statuses retrieved successfully"));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<GoalTrackerStatusResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<GoalTrackerStatusResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GoalTrackerStatusResponseDto>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<GoalTrackerStatusResponseDto>.ErrorResponse("Goal tracker status not found", null, 404));
        return Ok(ApiResponse<GoalTrackerStatusResponseDto>.SuccessResponse(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<GoalTrackerStatusResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<GoalTrackerStatusResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<GoalTrackerStatusResponseDto>>> Create([FromBody] CreateGoalTrackerStatusRequestDto request)
    {
        var actorId = await GetActorIdAsync();
        if (actorId == null)
            return Unauthorized(ApiResponse<GoalTrackerStatusResponseDto>.ErrorResponse("Unauthorized", null, 401));

        try
        {
            var created = await _service.CreateAsync(request, actorId.Value);
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                ApiResponse<GoalTrackerStatusResponseDto>.SuccessResponse(created, "Goal tracker status created successfully", 201));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating goal tracker status");
            return StatusCode(500, ApiResponse<GoalTrackerStatusResponseDto>.ErrorResponse("Internal server error", null, 500));
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<GoalTrackerStatusResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<GoalTrackerStatusResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GoalTrackerStatusResponseDto>>> Update(int id, [FromBody] UpdateGoalTrackerStatusRequestDto request)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, request);
            if (updated == null)
                return NotFound(ApiResponse<GoalTrackerStatusResponseDto>.ErrorResponse("Goal tracker status not found", null, 404));
            return Ok(ApiResponse<GoalTrackerStatusResponseDto>.SuccessResponse(updated, "Goal tracker status updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating goal tracker status {Id}", id);
            return StatusCode(500, ApiResponse<GoalTrackerStatusResponseDto>.ErrorResponse("Internal server error", null, 500));
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
                return NotFound(ApiResponse<object>.ErrorResponse("Goal tracker status not found", null, 404));
            return Ok(ApiResponse<object>.SuccessResponse(null, "Goal tracker status deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting goal tracker status {Id}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error", null, 500));
        }
    }

    [HttpPatch("{id:int}/toggle-active")]
    [ProducesResponseType(typeof(ApiResponse<GoalTrackerStatusResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<GoalTrackerStatusResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GoalTrackerStatusResponseDto>>> ToggleActive(int id)
    {
        try
        {
            var result = await _service.ToggleActiveAsync(id);
            if (result == null)
                return NotFound(ApiResponse<GoalTrackerStatusResponseDto>.ErrorResponse("Goal tracker status not found", null, 404));
            return Ok(ApiResponse<GoalTrackerStatusResponseDto>.SuccessResponse(result, "Goal tracker status toggled successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling goal tracker status {Id}", id);
            return StatusCode(500, ApiResponse<GoalTrackerStatusResponseDto>.ErrorResponse("Internal server error", null, 500));
        }
    }
}
