using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.Staff;
using TherapuHubAPI.DTOs.Responses.Staff;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;
    private readonly ILogger<StaffController> _logger;

    public StaffController(IStaffService staffService, ILogger<StaffController> logger)
    {
        _staffService = staffService;
        _logger = logger;
    }

    private int? GetCompanyId()
    {
        var claim = User.FindFirst("CompanyId");
        if (claim == null || !int.TryParse(claim.Value, out var companyId))
            return null;
        return companyId;
    }

    /// <summary>Get all staff for the current user's company.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StaffResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<StaffResponseDto>>>> GetAll()
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<IEnumerable<StaffResponseDto>>.ErrorResponse("CompanyId not found", null, 401));

        var result = await _staffService.GetByCompanyIdAsync(companyId.Value);
        return Ok(ApiResponse<IEnumerable<StaffResponseDto>>.SuccessResponse(result, "Staff retrieved successfully", 200));
    }

    /// <summary>Get staff by id (must belong to current company).</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<StaffResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<StaffResponseDto>>> GetById(int id)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<StaffResponseDto>.ErrorResponse("CompanyId not found", null, 401));

        var result = await _staffService.GetByIdAsync(id, companyId.Value);
        if (result == null)
            return NotFound(ApiResponse<StaffResponseDto>.NotFoundResponse($"Staff with Id {id} not found"));
        return Ok(ApiResponse<StaffResponseDto>.SuccessResponse(result, "Staff retrieved successfully", 200));
    }

    /// <summary>Get all staff statuses (for dropdowns).</summary>
    [HttpGet("statuses")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StaffStatusResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<StaffStatusResponseDto>>>> GetStatuses()
    {
        var result = await _staffService.GetAllStatusesAsync();
        return Ok(ApiResponse<IEnumerable<StaffStatusResponseDto>>.SuccessResponse(result, "Statuses retrieved", 200));
    }

    /// <summary>Get all staff roles (for dropdowns).</summary>
    [HttpGet("roles")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StaffRoleResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<StaffRoleResponseDto>>>> GetRoles()
    {
        var result = await _staffService.GetAllRolesAsync();
        return Ok(ApiResponse<IEnumerable<StaffRoleResponseDto>>.SuccessResponse(result, "Roles retrieved", 200));
    }

    /// <summary>Create a new staff member for the current company.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StaffResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<StaffResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<StaffResponseDto>>> Create([FromBody] CreateStaffRequestDto request)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<StaffResponseDto>.ErrorResponse("CompanyId not found", null, 401));

        try
        {
            var result = await _staffService.CreateAsync(request, companyId.Value);
            return CreatedAtAction(nameof(GetById), new { id = result.Id },
                ApiResponse<StaffResponseDto>.SuccessResponse(result, "Staff created successfully", 201));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error creating staff");
            return BadRequest(ApiResponse<StaffResponseDto>.ErrorResponse(ex.Message, new List<string> { ex.Message }, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating staff");
            return StatusCode(500, ApiResponse<StaffResponseDto>.ErrorResponse(
                "Internal server error", new List<string> { "An error occurred while processing the request" }, 500));
        }
    }

    /// <summary>Update an existing staff member.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<StaffResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<StaffResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<StaffResponseDto>>> Update(int id, [FromBody] UpdateStaffRequestDto request)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<StaffResponseDto>.ErrorResponse("CompanyId not found", null, 401));

        var result = await _staffService.UpdateAsync(id, request, companyId.Value);
        if (result == null)
            return NotFound(ApiResponse<StaffResponseDto>.NotFoundResponse($"Staff with Id {id} not found"));
        return Ok(ApiResponse<StaffResponseDto>.SuccessResponse(result, "Staff updated successfully", 200));
    }

    /// <summary>Delete a staff member.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<object>.ErrorResponse("CompanyId not found", null, 401));

        var success = await _staffService.DeleteAsync(id, companyId.Value);
        if (!success)
            return NotFound(ApiResponse<object>.NotFoundResponse($"Staff with Id {id} not found"));
        return Ok(ApiResponse<object>.SuccessResponse(null, "Staff deleted successfully", 200));
    }

    /// <summary>Toggle staff active/inactive.</summary>
    [HttpPatch("{id}/toggle-active")]
    [ProducesResponseType(typeof(ApiResponse<StaffResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<StaffResponseDto>>> ToggleActive(int id)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<StaffResponseDto>.ErrorResponse("CompanyId not found", null, 401));

        var success = await _staffService.ToggleActiveAsync(id, companyId.Value);
        if (!success)
            return NotFound(ApiResponse<StaffResponseDto>.NotFoundResponse($"Staff with Id {id} not found"));
        var updated = await _staffService.GetByIdAsync(id, companyId.Value);
        return Ok(ApiResponse<StaffResponseDto>.SuccessResponse(updated!, "Staff status updated", 200));
    }
}
