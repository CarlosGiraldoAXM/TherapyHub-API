using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Responses.Files;
using TherapuHubAPI.Models;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FilesController : ControllerBase
{
    private readonly IEntityFilesService _filesService;
    private readonly ContextDB _context;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IEntityFilesService filesService, ContextDB context, ILogger<FilesController> logger)
    {
        _filesService = filesService;
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
            .Where(u => u.Id == userId.Value)
            .Select(u => (int?)u.ActorId)
            .FirstOrDefaultAsync();
    }

    /// <summary>Get files for an entity by owner actor and file type.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EntityFileResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<EntityFileResponseDto>>>> GetByOwner(
        [FromQuery] int ownerActorId,
        [FromQuery] int filesTypeId)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<IEnumerable<EntityFileResponseDto>>.ErrorResponse("Unauthorized", null, 401));

        var result = await _filesService.GetByOwnerAndTypeAsync(ownerActorId, filesTypeId);
        return Ok(ApiResponse<IEnumerable<EntityFileResponseDto>>.SuccessResponse(result, "Files retrieved successfully", 200));
    }

    /// <summary>Upload a file for an entity (e.g. Competency Assessment).</summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<EntityFileResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<EntityFileResponseDto>>> Upload(
        [FromQuery] int ownerActorId,
        [FromQuery] int filesTypeId,
        [FromForm] UploadEntityFileRequest request)
    {
        var companyId = GetCompanyId();
        var actorId = await GetActorIdAsync();
        if (companyId == null || actorId == null)
            return Unauthorized(ApiResponse<EntityFileResponseDto>.ErrorResponse("Unauthorized", null, 401));

        var file = request.File;
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<EntityFileResponseDto>.ErrorResponse("No file provided", null, 400));

        try
        {
            var result = await _filesService.UploadAsync(file, ownerActorId, filesTypeId, actorId.Value, companyId.Value);
            return StatusCode(201, ApiResponse<EntityFileResponseDto>.SuccessResponse(result, "File uploaded successfully", 201));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<EntityFileResponseDto>.ErrorResponse(ex.Message, new List<string> { ex.Message }, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file for actor {OwnerActorId}", ownerActorId);
            return StatusCode(500, ApiResponse<EntityFileResponseDto>.ErrorResponse("Internal server error", null, 500));
        }
    }

    /// <summary>Download a file by ID.</summary>
    [HttpGet("{id:long}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Download(long id)
    {
        var companyId = GetCompanyId();
        if (companyId == null) return Unauthorized();

        var result = await _filesService.DownloadAsync(id);
        if (result == null)
            return NotFound(ApiResponse<object>.NotFoundResponse("File not found"));

        var (stream, contentType, fileName) = result.Value;
        return File(stream, contentType, fileName);
    }

    /// <summary>Soft-delete a file.</summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long id)
    {
        var actorId = await GetActorIdAsync();
        if (actorId == null)
            return Unauthorized(ApiResponse<object>.ErrorResponse("Unauthorized", null, 401));

        var success = await _filesService.DeleteAsync(id, actorId.Value);
        if (!success)
            return NotFound(ApiResponse<object>.NotFoundResponse("File not found"));

        return Ok(ApiResponse<object>.SuccessResponse(null, "File deleted successfully", 200));
    }
}

public class UploadEntityFileRequest
{
    public IFormFile File { get; set; } = null!;
}
