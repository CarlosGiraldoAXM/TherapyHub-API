using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.Companies;
using TherapuHubAPI.DTOs.Responses.Companies;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CompaniasController : ControllerBase
{
    private readonly ICompaniaService _companiaService;
    private readonly ILogger<CompaniasController> _logger;

    public CompaniasController(
        ICompaniaService companiaService,
        ILogger<CompaniasController> logger)
    {
        _companiaService = companiaService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new company
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CompaniaResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<CompaniaResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<CompaniaResponseDto>>> Create([FromBody] CreateCompaniaRequestDto request)
    {
        try
        {
            var result = await _companiaService.CreateAsync(request);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                ApiResponse<CompaniaResponseDto>.SuccessResponse(
                    result,
                    "Company created successfully",
                    201));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error when creating company");
            return BadRequest(ApiResponse<CompaniaResponseDto>.ErrorResponse(
                ex.Message,
                new List<string> { ex.Message },
                400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating company");
            return StatusCode(500, ApiResponse<CompaniaResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Gets all companies
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CompaniaResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CompaniaResponseDto>>>> GetAll()
    {
        try
        {
            var result = await _companiaService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<CompaniaResponseDto>>.SuccessResponse(
                result,
                "Companies retrieved successfully",
                200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving companies");
            return StatusCode(500, ApiResponse<IEnumerable<CompaniaResponseDto>>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Gets a company by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CompaniaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CompaniaResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CompaniaResponseDto>>> GetById(int id)
    {
        try
        {
            var result = await _companiaService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<CompaniaResponseDto>.NotFoundResponse(
                    $"Company with Id {id} not found"));
            }
            return Ok(ApiResponse<CompaniaResponseDto>.SuccessResponse(
                result,
                "Company retrieved successfully",
                200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company with Id: {Id}", id);
            return StatusCode(500, ApiResponse<CompaniaResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Updates an existing company
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CompaniaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CompaniaResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CompaniaResponseDto>>> Update(int id, [FromBody] UpdateCompaniaRequestDto request)
    {
        try
        {
            var result = await _companiaService.UpdateAsync(id, request);
            if (result == null)
            {
                return NotFound(ApiResponse<CompaniaResponseDto>.NotFoundResponse(
                    $"Company with Id {id} not found"));
            }
            return Ok(ApiResponse<CompaniaResponseDto>.SuccessResponse(
                result,
                "Company updated successfully",
                200));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error when updating company with Id: {Id}", id);
            return BadRequest(ApiResponse<CompaniaResponseDto>.ErrorResponse(
                ex.Message,
                new List<string> { ex.Message },
                400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company with Id: {Id}", id);
            return StatusCode(500, ApiResponse<CompaniaResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Deletes a company (soft delete). Fails if the company has active users assigned.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse(
                    "User could not be determined. Please log in again.",
                    new List<string> { "Invalid or missing user context" },
                    401));
            }

            var result = await _companiaService.DeleteAsync(id, currentUserId);
            if (!result)
            {
                return NotFound(ApiResponse<object>.NotFoundResponse(
                    $"Company with Id {id} not found"));
            }
            return Ok(ApiResponse<object>.SuccessResponse(
                null,
                "Company deleted successfully",
                200));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error when deleting company with Id: {Id}", id);
            return Conflict(ApiResponse<object>.ErrorResponse(
                ex.Message,
                new List<string> { ex.Message },
                409));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting company with Id: {Id}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Toggles the active/inactive status of a company
    /// </summary>
    [HttpPatch("{id}/toggle-activo")]
    [ProducesResponseType(typeof(ApiResponse<CompaniaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CompaniaResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CompaniaResponseDto>>> ToggleActivo(int id)
    {
        try
        {
            var result = await _companiaService.ToggleActivoAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<CompaniaResponseDto>.NotFoundResponse(
                    $"Company with Id {id} not found"));
            }
            var compania = await _companiaService.GetByIdAsync(id);
            return Ok(ApiResponse<CompaniaResponseDto>.SuccessResponse(
                compania!,
                "Company status updated successfully",
                200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling company status with Id: {Id}", id);
            return StatusCode(500, ApiResponse<CompaniaResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }
}
