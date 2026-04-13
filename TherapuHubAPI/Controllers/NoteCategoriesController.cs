using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.NoteCategories;
using TherapuHubAPI.DTOs.Responses.NoteCategories;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NoteCategoriesController : ControllerBase
{
    private readonly INoteCategoryService _service;
    private readonly ILogger<NoteCategoriesController> _logger;

    public NoteCategoriesController(INoteCategoryService service, ILogger<NoteCategoriesController> logger)
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

    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null || !int.TryParse(claim.Value, out var id)) return null;
        return id;
    }

    /// <summary>Get all categories for a given section.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NoteCategoryResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<NoteCategoryResponseDto>>>> GetBySection([FromQuery] int sectionId)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<IEnumerable<NoteCategoryResponseDto>>.ErrorResponse("CompanyId not found", null, 401));

        var result = await _service.GetBySectionAsync(companyId.Value, sectionId);
        return Ok(ApiResponse<IEnumerable<NoteCategoryResponseDto>>.SuccessResponse(result, "Categories retrieved", 200));
    }

    /// <summary>Get a single category by id.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<NoteCategoryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteCategoryResponseDto>>> GetById(int id)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<NoteCategoryResponseDto>.ErrorResponse("CompanyId not found", null, 401));

        var result = await _service.GetByIdAsync(id, companyId.Value);
        if (result == null)
            return NotFound(ApiResponse<NoteCategoryResponseDto>.NotFoundResponse($"Category {id} not found"));
        return Ok(ApiResponse<NoteCategoryResponseDto>.SuccessResponse(result, "Category retrieved", 200));
    }

    /// <summary>Create a new category.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<NoteCategoryResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteCategoryResponseDto>>> Create([FromBody] CreateNoteCategoryRequestDto dto)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<NoteCategoryResponseDto>.ErrorResponse("CompanyId not found", null, 401));

        var actorId = GetUserId();
        if (actorId == null)
            return Unauthorized(ApiResponse<NoteCategoryResponseDto>.ErrorResponse("User not found", null, 401));

        var result = await _service.CreateAsync(dto, companyId.Value, actorId.Value);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<NoteCategoryResponseDto>.SuccessResponse(result, "Category created", 201));
    }

    /// <summary>Update a category.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<NoteCategoryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteCategoryResponseDto>>> Update(int id, [FromBody] UpdateNoteCategoryRequestDto dto)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<NoteCategoryResponseDto>.ErrorResponse("CompanyId not found", null, 401));

        var result = await _service.UpdateAsync(id, dto, companyId.Value);
        if (result == null)
            return NotFound(ApiResponse<NoteCategoryResponseDto>.NotFoundResponse($"Category {id} not found"));
        return Ok(ApiResponse<NoteCategoryResponseDto>.SuccessResponse(result, "Category updated", 200));
    }

    /// <summary>Delete a category.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<object>.ErrorResponse("CompanyId not found", null, 401));

        var success = await _service.DeleteAsync(id, companyId.Value);
        if (!success)
            return NotFound(ApiResponse<object>.NotFoundResponse($"Category {id} not found"));
        return Ok(ApiResponse<object>.SuccessResponse(null, "Category deleted", 200));
    }

    /// <summary>Toggle category active/inactive.</summary>
    [HttpPatch("{id}/toggle-active")]
    [ProducesResponseType(typeof(ApiResponse<NoteCategoryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteCategoryResponseDto>>> ToggleActive(int id)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<NoteCategoryResponseDto>.ErrorResponse("CompanyId not found", null, 401));

        var result = await _service.ToggleActiveAsync(id, companyId.Value);
        if (result == null)
            return NotFound(ApiResponse<NoteCategoryResponseDto>.NotFoundResponse($"Category {id} not found"));
        return Ok(ApiResponse<NoteCategoryResponseDto>.SuccessResponse(result, "Category status toggled", 200));
    }
}
