using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.ActorRelationships;
using TherapuHubAPI.DTOs.Responses.ActorRelationships;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ActorRelationshipsController : ControllerBase
{
    private readonly IActorRelationshipService _service;
    private readonly ILogger<ActorRelationshipsController> _logger;

    public ActorRelationshipsController(IActorRelationshipService service, ILogger<ActorRelationshipsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    private int? GetCompanyId()
    {
        var claim = User.FindFirst("CompanyId");
        if (claim == null || !int.TryParse(claim.Value, out var id)) return null;
        return id;
    }

    // ─── Supervisor → RBT ────────────────────────────────────────────────────

    /// <summary>Non-system users in the company with their type-1 RBT assignment count.</summary>
    [HttpGet("supervisors")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SupervisorResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SupervisorResponseDto>>>> GetSupervisors()
    {
        var companyId = GetCompanyId();
        if (companyId == null) return Unauthorized(ApiResponse<object>.ErrorResponse("CompanyId not found", null, 401));

        var result = await _service.GetSupervisorsAsync(companyId.Value);
        return Ok(ApiResponse<IEnumerable<SupervisorResponseDto>>.SuccessResponse(result, "Supervisors retrieved", 200));
    }

    /// <summary>All RBTs in the company with assignment status for the given supervisor.</summary>
    [HttpGet("{supervisorActorId}/rbts")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<RbtAssignmentResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<RbtAssignmentResponseDto>>>> GetRbtsForSupervisor(int supervisorActorId)
    {
        var companyId = GetCompanyId();
        if (companyId == null) return Unauthorized(ApiResponse<object>.ErrorResponse("CompanyId not found", null, 401));

        var result = await _service.GetRbtsForSupervisorAsync(supervisorActorId, companyId.Value);
        return Ok(ApiResponse<IEnumerable<RbtAssignmentResponseDto>>.SuccessResponse(result, "RBTs retrieved", 200));
    }

    /// <summary>Assign an RBT to a supervisor (type 1).</summary>
    [HttpPost("rbts")]
    [ProducesResponseType(typeof(ApiResponse<ActorRelationshipResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ActorRelationshipResponseDto>>> AssignRbt([FromBody] AssignRbtRequestDto request)
    {
        var companyId = GetCompanyId();
        if (companyId == null) return Unauthorized(ApiResponse<object>.ErrorResponse("CompanyId not found", null, 401));

        try
        {
            var result = await _service.AssignRbtAsync(request, companyId.Value);
            return StatusCode(201, ApiResponse<ActorRelationshipResponseDto>.SuccessResponse(result, "RBT assigned", 201));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, new List<string> { ex.Message }, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning RBT");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error", null, 500));
        }
    }

    // ─── User → User ─────────────────────────────────────────────────────────

    /// <summary>All users in the company with their type-2 delegate assignment count.</summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SupervisorResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SupervisorResponseDto>>>> GetUserSources()
    {
        var companyId = GetCompanyId();
        if (companyId == null) return Unauthorized(ApiResponse<object>.ErrorResponse("CompanyId not found", null, 401));

        var result = await _service.GetUserSourcesAsync(companyId.Value);
        return Ok(ApiResponse<IEnumerable<SupervisorResponseDto>>.SuccessResponse(result, "Users retrieved", 200));
    }

    /// <summary>All other users in the company with assignment status for the given source user.</summary>
    [HttpGet("{sourceActorId}/assigned-users")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserAssignmentResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserAssignmentResponseDto>>>> GetUsersForSource(int sourceActorId)
    {
        var companyId = GetCompanyId();
        if (companyId == null) return Unauthorized(ApiResponse<object>.ErrorResponse("CompanyId not found", null, 401));

        var result = await _service.GetUsersForSourceAsync(sourceActorId, companyId.Value);
        return Ok(ApiResponse<IEnumerable<UserAssignmentResponseDto>>.SuccessResponse(result, "Users retrieved", 200));
    }

    /// <summary>Assign a user to another user (type 2).</summary>
    [HttpPost("users")]
    [ProducesResponseType(typeof(ApiResponse<ActorRelationshipResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ActorRelationshipResponseDto>>> AssignUser([FromBody] AssignUserRequestDto request)
    {
        var companyId = GetCompanyId();
        if (companyId == null) return Unauthorized(ApiResponse<object>.ErrorResponse("CompanyId not found", null, 401));

        try
        {
            var result = await _service.AssignUserAsync(request, companyId.Value);
            return StatusCode(201, ApiResponse<ActorRelationshipResponseDto>.SuccessResponse(result, "User assigned", 201));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, new List<string> { ex.Message }, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning user");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error", null, 500));
        }
    }

    // ─── RBT → Client (type 3) ────────────────────────────────────────────────

    /// <summary>All clients in the company with assignment status for the given RBT.</summary>
    [HttpGet("{rbtActorId}/clients")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClientAssignmentResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClientAssignmentResponseDto>>>> GetClientsForRbt(int rbtActorId)
    {
        var companyId = GetCompanyId();
        if (companyId == null) return Unauthorized(ApiResponse<object>.ErrorResponse("CompanyId not found", null, 401));

        var result = await _service.GetClientsForRbtAsync(rbtActorId, companyId.Value);
        return Ok(ApiResponse<IEnumerable<ClientAssignmentResponseDto>>.SuccessResponse(result, "Clients retrieved", 200));
    }

    /// <summary>Assign a client to an RBT (type 3).</summary>
    [HttpPost("clients")]
    [ProducesResponseType(typeof(ApiResponse<ActorRelationshipResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ActorRelationshipResponseDto>>> AssignClient([FromBody] AssignClientRequestDto request)
    {
        var companyId = GetCompanyId();
        if (companyId == null) return Unauthorized(ApiResponse<object>.ErrorResponse("CompanyId not found", null, 401));

        try
        {
            var result = await _service.AssignClientAsync(request, companyId.Value);
            return StatusCode(201, ApiResponse<ActorRelationshipResponseDto>.SuccessResponse(result, "Client assigned", 201));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, new List<string> { ex.Message }, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning client");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error", null, 500));
        }
    }

    // ─── Shared ───────────────────────────────────────────────────────────────

    /// <summary>Remove any relationship by id.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> RemoveAssignment(long id)
    {
        var companyId = GetCompanyId();
        if (companyId == null) return Unauthorized(ApiResponse<object>.ErrorResponse("CompanyId not found", null, 401));

        var success = await _service.RemoveAssignmentAsync(id, companyId.Value);
        if (!success) return NotFound(ApiResponse<object>.NotFoundResponse($"Assignment {id} not found"));

        return Ok(ApiResponse<object>.SuccessResponse(null, "Assignment removed", 200));
    }
}
