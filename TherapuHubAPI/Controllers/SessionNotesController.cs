using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.SessionNotes;
using TherapuHubAPI.DTOs.Responses.SessionNotes;
using TherapuHubAPI.Models;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SessionNotesController : ControllerBase
{
    private readonly ISessionNotesService _service;
    private readonly ContextDB _context;
    private readonly ILogger<SessionNotesController> _logger;

    public SessionNotesController(ISessionNotesService service, ContextDB context, ILogger<SessionNotesController> logger)
    {
        _service = service;
        _context = context;
        _logger = logger;
    }

    private int? GetCompanyId()
    {
        var claim = User.FindFirst("CompanyId");
        if (claim == null || !int.TryParse(claim.Value, out var id)) return null;
        return id;
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
            .Where(u => u.Id == userId)
            .Select(u => (int?)u.ActorId)
            .FirstOrDefaultAsync();
    }

    /// <summary>Get RBTs visible to the current user (via ActorRelationships, or all for system users).</summary>
    [HttpGet("rbts")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<RbtForSessionResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<RbtForSessionResponseDto>>>> GetRbts()
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();
        if (companyId == null || userId == null)
            return Unauthorized(ApiResponse<IEnumerable<RbtForSessionResponseDto>>.ErrorResponse("Unauthorized", null, 401));

        var result = await _service.GetRbtsForUserAsync(userId.Value, companyId.Value);
        return Ok(ApiResponse<IEnumerable<RbtForSessionResponseDto>>.SuccessResponse(result, "RBTs retrieved", 200));
    }

    /// <summary>Get active session note statuses.</summary>
    [HttpGet("statuses")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SessionNoteStatusResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SessionNoteStatusResponseDto>>>> GetStatuses()
    {
        var result = await _service.GetStatusesAsync();
        return Ok(ApiResponse<IEnumerable<SessionNoteStatusResponseDto>>.SuccessResponse(result, "Statuses retrieved", 200));
    }

    /// <summary>Get session notes for a date range (typically a week).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SessionNoteResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SessionNoteResponseDto>>>> GetByWeek(
        [FromQuery] string weekStart,
        [FromQuery] string weekEnd)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<IEnumerable<SessionNoteResponseDto>>.ErrorResponse("Unauthorized", null, 401));

        if (!DateTime.TryParse(weekStart, out var start) || !DateTime.TryParse(weekEnd, out var end))
            return BadRequest(ApiResponse<IEnumerable<SessionNoteResponseDto>>.ErrorResponse("Invalid date range", null, 400));

        var result = await _service.GetByWeekAsync(companyId.Value, start, end);
        return Ok(ApiResponse<IEnumerable<SessionNoteResponseDto>>.SuccessResponse(result, "Session notes retrieved", 200));
    }

    /// <summary>Create a new session note.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SessionNoteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<SessionNoteResponseDto>>> Create([FromBody] CreateSessionNoteRequestDto dto)
    {
        var actorId = await GetActorIdAsync();
        if (actorId == null)
            return Unauthorized(ApiResponse<SessionNoteResponseDto>.ErrorResponse("Unauthorized", null, 401));

        var result = await _service.CreateAsync(dto, actorId.Value);
        return Ok(ApiResponse<SessionNoteResponseDto>.SuccessResponse(result, "Session note created", 201));
    }

    /// <summary>Update an existing session note.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SessionNoteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SessionNoteResponseDto>>> Update(long id, [FromBody] UpdateSessionNoteRequestDto dto)
    {
        var actorId = await GetActorIdAsync();
        if (actorId == null)
            return Unauthorized(ApiResponse<SessionNoteResponseDto>.ErrorResponse("Unauthorized", null, 401));

        var result = await _service.UpdateAsync(id, dto, actorId.Value);
        if (result == null)
            return NotFound(ApiResponse<SessionNoteResponseDto>.NotFoundResponse($"Session note {id} not found"));

        return Ok(ApiResponse<SessionNoteResponseDto>.SuccessResponse(result, "Session note updated", 200));
    }

    /// <summary>Soft-delete a session note.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long id)
    {
        var actorId = await GetActorIdAsync();
        if (actorId == null)
            return Unauthorized(ApiResponse<object>.ErrorResponse("Unauthorized", null, 401));

        var deleted = await _service.DeleteAsync(id, actorId.Value);
        if (!deleted)
            return NotFound(ApiResponse<object>.NotFoundResponse($"Session note {id} not found"));

        return Ok(ApiResponse<object>.SuccessResponse(null, "Session note deleted", 200));
    }
}
