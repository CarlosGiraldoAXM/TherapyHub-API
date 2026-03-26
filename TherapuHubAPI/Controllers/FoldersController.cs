using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.Folders;
using TherapuHubAPI.DTOs.Responses.Folders;
using TherapuHubAPI.Models;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FoldersController : ControllerBase
{
    private readonly IFolderService _folderService;
    private readonly ContextDB _context;
    private readonly ILogger<FoldersController> _logger;

    public FoldersController(IFolderService folderService, ContextDB context, ILogger<FoldersController> logger)
    {
        _folderService = folderService;
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

    private int? GetUserTypeId()
    {
        var claim = User.FindFirst("UserTypeId");
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

    // ─── Folders ─────────────────────────────────────────────────────────────

    /// <summary>Get all folders of a specific type for the current company.</summary>
    /// <param name="folderTypeId">FolderType ID (1=Staff, 2=Clinical, 3=Education, 4=Supervision, 5=Documents, 6=Library)</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FolderResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<FolderResponseDto>>>> GetAll([FromQuery] byte folderTypeId)
    {
        var companyId = GetCompanyId();
        var userTypeId = GetUserTypeId();
        var actorId = await GetActorIdAsync();
        if (companyId == null || actorId == null || userTypeId == null)
            return Unauthorized(ApiResponse<IEnumerable<FolderResponseDto>>.ErrorResponse("Unauthorized", null, 401));

        var result = await _folderService.GetFoldersByTypeAsync(companyId.Value, folderTypeId, actorId.Value, userTypeId.Value);
        return Ok(ApiResponse<IEnumerable<FolderResponseDto>>.SuccessResponse(result, "Folders retrieved successfully", 200));
    }

    /// <summary>Get all direct subfolders of a folder.</summary>
    [HttpGet("{parentFolderId:int}/subfolders")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FolderResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<FolderResponseDto>>>> GetSubfolders(int parentFolderId)
    {
        var companyId = GetCompanyId();
        var userTypeId = GetUserTypeId();
        var actorId = await GetActorIdAsync();
        if (companyId == null || actorId == null || userTypeId == null)
            return Unauthorized(ApiResponse<IEnumerable<FolderResponseDto>>.ErrorResponse("Unauthorized", null, 401));

        var result = await _folderService.GetSubfoldersAsync(parentFolderId, companyId.Value, actorId.Value, userTypeId.Value);
        return Ok(ApiResponse<IEnumerable<FolderResponseDto>>.SuccessResponse(result, "Subfolders retrieved successfully", 200));
    }

    /// <summary>Get a single folder by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<FolderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<FolderResponseDto>>> GetById(int id)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<FolderResponseDto>.ErrorResponse("CompanyId not found", null, 401));

        var result = await _folderService.GetFolderByIdAsync(id, companyId.Value);
        if (result == null)
            return NotFound(ApiResponse<FolderResponseDto>.NotFoundResponse($"Folder with Id {id} not found"));

        return Ok(ApiResponse<FolderResponseDto>.SuccessResponse(result, "Folder retrieved successfully", 200));
    }

    /// <summary>Create a new folder.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FolderResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<FolderResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<FolderResponseDto>>> Create([FromBody] CreateFolderRequestDto request)
    {
        var companyId = GetCompanyId();
        var userTypeId = GetUserTypeId();
        var actorId = await GetActorIdAsync();
        if (companyId == null || actorId == null || userTypeId == null)
            return Unauthorized(ApiResponse<FolderResponseDto>.ErrorResponse("Unauthorized", null, 401));

        try
        {
            var result = await _folderService.CreateFolderAsync(request, companyId.Value, actorId.Value, userTypeId.Value);
            return CreatedAtAction(nameof(GetById), new { id = result.Id },
                ApiResponse<FolderResponseDto>.SuccessResponse(result, "Folder created successfully", 201));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<FolderResponseDto>.ErrorResponse(ex.Message, new List<string> { ex.Message }, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating folder");
            return StatusCode(500, ApiResponse<FolderResponseDto>.ErrorResponse("Internal server error", null, 500));
        }
    }

    /// <summary>Update an existing folder.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<FolderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<FolderResponseDto>>> Update(int id, [FromBody] UpdateFolderRequestDto request)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<FolderResponseDto>.ErrorResponse("CompanyId not found", null, 401));

        try
        {
            var result = await _folderService.UpdateFolderAsync(id, request, companyId.Value);
            if (result == null)
                return NotFound(ApiResponse<FolderResponseDto>.NotFoundResponse($"Folder with Id {id} not found"));

            return Ok(ApiResponse<FolderResponseDto>.SuccessResponse(result, "Folder updated successfully", 200));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<FolderResponseDto>.ErrorResponse(ex.Message, new List<string> { ex.Message }, 400));
        }
    }

    /// <summary>Delete a folder and all its files. Only the owner or a system user can delete.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var companyId = GetCompanyId();
        var userTypeId = GetUserTypeId();
        var actorId = await GetActorIdAsync();
        if (companyId == null || actorId == null || userTypeId == null)
            return Unauthorized(ApiResponse<object>.ErrorResponse("Unauthorized", null, 401));

        try
        {
            var success = await _folderService.DeleteFolderAsync(id, companyId.Value, actorId.Value, userTypeId.Value);
            if (!success)
                return NotFound(ApiResponse<object>.NotFoundResponse($"Folder with Id {id} not found"));

            return Ok(ApiResponse<object>.SuccessResponse(null, "Folder deleted successfully", 200));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ApiResponse<object>.ErrorResponse(ex.Message, new List<string> { ex.Message }, 403));
        }
    }

    // ─── Files ────────────────────────────────────────────────────────────────

    /// <summary>List all files in a folder.</summary>
    [HttpGet("{folderId:int}/files")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FileResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<FileResponseDto>>>> GetFiles(int folderId)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<IEnumerable<FileResponseDto>>.ErrorResponse("CompanyId not found", null, 401));

        var result = await _folderService.GetFilesAsync(folderId, companyId.Value);
        return Ok(ApiResponse<IEnumerable<FileResponseDto>>.SuccessResponse(result, "Files retrieved successfully", 200));
    }

    /// <summary>Upload one or more files to a folder.</summary>
    [HttpPost("{folderId:int}/files")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FileResponseDto>>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<FileResponseDto>>>> UploadFiles(int folderId, [FromForm] IFormFileCollection files)
    {
        var companyId = GetCompanyId();
        var actorId = await GetActorIdAsync();
        if (companyId == null || actorId == null)
            return Unauthorized(ApiResponse<IEnumerable<FileResponseDto>>.ErrorResponse("Unauthorized", null, 401));

        if (files == null || files.Count == 0)
            return BadRequest(ApiResponse<IEnumerable<FileResponseDto>>.ErrorResponse("No files provided", null, 400));

        try
        {
            var uploaded = new List<FileResponseDto>();
            foreach (var file in files)
            {
                var result = await _folderService.UploadFileAsync(folderId, file, companyId.Value, actorId.Value);
                uploaded.Add(result);
            }

            return StatusCode(201, ApiResponse<IEnumerable<FileResponseDto>>.SuccessResponse(uploaded, "Files uploaded successfully", 201));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<FileResponseDto>>.ErrorResponse(ex.Message, new List<string> { ex.Message }, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading files to folder {FolderId}", folderId);
            return StatusCode(500, ApiResponse<IEnumerable<FileResponseDto>>.ErrorResponse("Internal server error", null, 500));
        }
    }

    /// <summary>Download a file.</summary>
    [HttpGet("{folderId:int}/files/{fileId:long}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DownloadFile(int folderId, long fileId)
    {
        var companyId = GetCompanyId();
        if (companyId == null) return Unauthorized();

        var result = await _folderService.DownloadFileAsync(fileId, folderId, companyId.Value);
        if (result == null)
            return NotFound(ApiResponse<object>.NotFoundResponse("File not found"));

        var (stream, contentType, fileName) = result.Value;
        return File(stream, contentType, fileName);
    }

    /// <summary>Delete a file from a folder.</summary>
    [HttpDelete("{folderId:int}/files/{fileId:long}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteFile(int folderId, long fileId)
    {
        var companyId = GetCompanyId();
        var actorId = await GetActorIdAsync();
        if (companyId == null || actorId == null)
            return Unauthorized(ApiResponse<object>.ErrorResponse("Unauthorized", null, 401));

        var success = await _folderService.DeleteFileAsync(fileId, folderId, companyId.Value, actorId.Value);
        if (!success)
            return NotFound(ApiResponse<object>.NotFoundResponse("File not found"));

        return Ok(ApiResponse<object>.SuccessResponse(null, "File deleted successfully", 200));
    }
}
