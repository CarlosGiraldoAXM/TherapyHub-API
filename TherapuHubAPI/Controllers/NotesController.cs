using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.Notes;
using TherapuHubAPI.DTOs.Responses.Notes;
using TherapuHubAPI.Models;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NotesController : ControllerBase
{
    private readonly INotesService _notesService;
    private readonly ContextDB _context;
    private readonly ILogger<NotesController> _logger;

    public NotesController(INotesService notesService, ContextDB context, ILogger<NotesController> logger)
    {
        _notesService = notesService;
        _context = context;
        _logger = logger;
    }

    private int? GetCompanyId()
    {
        var claim = User.FindFirst("CompanyId");
        if (claim == null || !int.TryParse(claim.Value, out var companyId))
            return null;
        return companyId;
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null || !int.TryParse(claim.Value, out var userId))
            return null;
        return userId;
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

    /// <summary>Get all priorities (for dropdowns).</summary>
    [HttpGet("priorities")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NotePriorityResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<NotePriorityResponseDto>>>> GetPriorities()
    {
        var result = await _notesService.GetPrioritiesAsync();
        return Ok(ApiResponse<IEnumerable<NotePriorityResponseDto>>.SuccessResponse(result, "Priorities retrieved", 200));
    }

    /// <summary>
    /// Get notes for a menu filtered by owner actor.
    /// If ownerActorId is omitted, defaults to the current user's own ActorId (personal notes).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NoteResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<NoteResponseDto>>>> GetByOwner(
        [FromQuery] int menuId,
        [FromQuery] int? ownerActorId,
        [FromQuery] int? sectionId)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<IEnumerable<NoteResponseDto>>.ErrorResponse("CompanyId not found", null, 401));

        var currentActorId = await GetActorIdAsync();
        if (currentActorId == null)
            return Unauthorized(ApiResponse<IEnumerable<NoteResponseDto>>.ErrorResponse("Actor not found", null, 401));

        int resolvedOwnerActorId = (ownerActorId.HasValue && ownerActorId.Value != 0)
            ? ownerActorId.Value
            : currentActorId.Value;

        var result = await _notesService.GetByOwnerAsync(companyId.Value, menuId, resolvedOwnerActorId, currentActorId.Value, sectionId);
        return Ok(ApiResponse<IEnumerable<NoteResponseDto>>.SuccessResponse(result, "Notes retrieved successfully", 200));
    }

    /// <summary>Get note by id.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<NoteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> GetById(long id)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<NoteResponseDto>.ErrorResponse("CompanyId not found", null, 401));

        var result = await _notesService.GetByIdAsync(id, companyId.Value);
        if (result == null)
            return NotFound(ApiResponse<NoteResponseDto>.NotFoundResponse($"Note with Id {id} not found"));
        return Ok(ApiResponse<NoteResponseDto>.SuccessResponse(result, "Note retrieved successfully", 200));
    }

    /// <summary>Create a new note. The entityId in the body is resolved to a NoteSection automatically.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<NoteResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<NoteResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> Create([FromBody] CreateNoteRequestDto request)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<NoteResponseDto>.ErrorResponse("CompanyId not found", null, 401));

        var actorId = await GetActorIdAsync();
        if (actorId == null)
            return Unauthorized(ApiResponse<NoteResponseDto>.ErrorResponse("Actor not found", null, 401));

        try
        {
            var result = await _notesService.CreateAsync(request, companyId.Value, actorId.Value);
            return CreatedAtAction(nameof(GetById), new { id = result.Id },
                ApiResponse<NoteResponseDto>.SuccessResponse(result, "Note created successfully", 201));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error creating note");
            return BadRequest(ApiResponse<NoteResponseDto>.ErrorResponse(ex.Message, new List<string> { ex.Message }, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating note");
            return StatusCode(500, ApiResponse<NoteResponseDto>.ErrorResponse(
                "Internal server error", new List<string> { "An error occurred while processing the request" }, 500));
        }
    }

    /// <summary>Update an existing note.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<NoteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> Update(long id, [FromBody] UpdateNoteRequestDto request)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<NoteResponseDto>.ErrorResponse("CompanyId not found", null, 401));

        try
        {
            var result = await _notesService.UpdateAsync(id, request, companyId.Value);
            if (result == null)
                return NotFound(ApiResponse<NoteResponseDto>.NotFoundResponse($"Note with Id {id} not found"));
            return Ok(ApiResponse<NoteResponseDto>.SuccessResponse(result, "Note updated successfully", 200));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error updating note");
            return BadRequest(ApiResponse<NoteResponseDto>.ErrorResponse(ex.Message, new List<string> { ex.Message }, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating note");
            return StatusCode(500, ApiResponse<NoteResponseDto>.ErrorResponse(
                "Internal server error", new List<string> { "An error occurred while processing the request" }, 500));
        }
    }

    /// <summary>Delete a note.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long id)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<object>.ErrorResponse("CompanyId not found", null, 401));

        var success = await _notesService.DeleteAsync(id, companyId.Value);
        if (!success)
            return NotFound(ApiResponse<object>.NotFoundResponse($"Note with Id {id} not found"));
        return Ok(ApiResponse<object>.SuccessResponse(null, "Note deleted successfully", 200));
    }

    /// <summary>Toggle note active/inactive (marks as completed or not).</summary>
    [HttpPatch("{id}/toggle-active")]
    [ProducesResponseType(typeof(ApiResponse<NoteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> ToggleActive(long id)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<NoteResponseDto>.ErrorResponse("CompanyId not found", null, 401));

        var success = await _notesService.ToggleActiveAsync(id, companyId.Value);
        if (!success)
            return NotFound(ApiResponse<NoteResponseDto>.NotFoundResponse($"Note with Id {id} not found"));

        var updated = await _notesService.GetByIdAsync(id, companyId.Value);
        return Ok(ApiResponse<NoteResponseDto>.SuccessResponse(updated!, "Note status updated", 200));
    }
}
