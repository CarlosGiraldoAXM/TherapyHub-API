using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.GoalTracker;
using TherapuHubAPI.DTOs.Responses.GoalTracker;
using TherapuHubAPI.Models;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GoalTrackerController : ControllerBase
{
    private readonly IGoalTrackerService _service;
    private readonly ContextDB _context;
    private readonly ILogger<GoalTrackerController> _logger;

    public GoalTrackerController(
        IGoalTrackerService service,
        ContextDB context,
        ILogger<GoalTrackerController> logger)
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

    [HttpGet("categories")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<GoalTrackerCategoryResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<GoalTrackerCategoryResponseDto>>>> GetCategories()
    {
        var result = await _service.GetCategoriesAsync();
        return Ok(ApiResponse<IEnumerable<GoalTrackerCategoryResponseDto>>.SuccessResponse(result, "Categories retrieved successfully"));
    }

    [HttpGet("client/{ownerActorId:int}")]
    [ProducesResponseType(typeof(ApiResponse<GoalTrackerResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<GoalTrackerResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GoalTrackerResponseDto>>> GetByOwner(int ownerActorId)
    {
        var result = await _service.GetByOwnerAsync(ownerActorId);
        if (result == null)
            return NotFound(ApiResponse<GoalTrackerResponseDto>.ErrorResponse("No goal tracker found for this client", null, 404));
        return Ok(ApiResponse<GoalTrackerResponseDto>.SuccessResponse(result));
    }

    [HttpPost("row")]
    [ProducesResponseType(typeof(ApiResponse<GoalTrackerResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<GoalTrackerResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<GoalTrackerResponseDto>>> CreateRow([FromBody] CreateGoalTrackerRowRequestDto request)
    {
        var actorId = await GetActorIdAsync();
        if (actorId == null)
            return Unauthorized(ApiResponse<GoalTrackerResponseDto>.ErrorResponse("Unauthorized", null, 401));

        try
        {
            var result = await _service.CreateRowAsync(request, actorId.Value);
            return StatusCode(201, ApiResponse<GoalTrackerResponseDto>.SuccessResponse(result, "Row created successfully", 201));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating goal tracker row");
            return StatusCode(500, ApiResponse<GoalTrackerResponseDto>.ErrorResponse("Internal server error", null, 500));
        }
    }

    [HttpPut("items/{itemId:long}")]
    [ProducesResponseType(typeof(ApiResponse<GoalTrackerItemResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<GoalTrackerItemResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GoalTrackerItemResponseDto>>> UpdateItem(long itemId, [FromBody] UpdateGoalTrackerItemRequestDto request)
    {
        try
        {
            var result = await _service.UpdateItemAsync(itemId, request);
            if (result == null)
                return NotFound(ApiResponse<GoalTrackerItemResponseDto>.ErrorResponse("Item not found", null, 404));
            return Ok(ApiResponse<GoalTrackerItemResponseDto>.SuccessResponse(result, "Item updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating goal tracker item {ItemId}", itemId);
            return StatusCode(500, ApiResponse<GoalTrackerItemResponseDto>.ErrorResponse("Internal server error", null, 500));
        }
    }

    [HttpDelete("items/{itemId:long}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteItem(long itemId)
    {
        try
        {
            var deleted = await _service.DeleteItemAsync(itemId);
            if (!deleted)
                return NotFound(ApiResponse<object>.ErrorResponse("Item not found", null, 404));
            return Ok(ApiResponse<object>.SuccessResponse(null, "Item deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting goal tracker item {ItemId}", itemId);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error", null, 500));
        }
    }
}
