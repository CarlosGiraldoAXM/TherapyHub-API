using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Responses.Staff;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/Staff/{staffId}/documents")]
[Produces("application/json")]
public class StaffDocumentsController : ControllerBase
{
    private readonly IStaffDocumentService _service;
    private readonly ILogger<StaffDocumentsController> _logger;

    public StaffDocumentsController(IStaffDocumentService service, ILogger<StaffDocumentsController> logger)
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

    /// <summary>Get all active documents for a staff member.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StaffDocumentResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<StaffDocumentResponseDto>>>> GetDocuments(int staffId)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<IEnumerable<StaffDocumentResponseDto>>.ErrorResponse("Unauthorized", null, 401));

        try
        {
            var docs = await _service.GetDocumentsByStaffAsync(staffId, companyId.Value);
            return Ok(ApiResponse<IEnumerable<StaffDocumentResponseDto>>.SuccessResponse(docs));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<IEnumerable<StaffDocumentResponseDto>>.NotFoundResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting documents for staff {StaffId}", staffId);
            return StatusCode(500, ApiResponse<IEnumerable<StaffDocumentResponseDto>>.ErrorResponse("Internal server error", null, 500));
        }
    }

    /// <summary>Upload a document for a staff member. documentTypeId=1 for Competency Assessment.</summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<StaffDocumentResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<StaffDocumentResponseDto>>> UploadDocument(
        int staffId,
        IFormFile file,
        [FromForm] byte documentTypeId = 1)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<StaffDocumentResponseDto>.ErrorResponse("Unauthorized", null, 401));

        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<StaffDocumentResponseDto>.ErrorResponse("No file provided."));

        try
        {
            var doc = await _service.UploadDocumentAsync(staffId, companyId.Value, file, documentTypeId);
            return StatusCode(201, ApiResponse<StaffDocumentResponseDto>.SuccessResponse(doc, "Document uploaded successfully", 201));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<StaffDocumentResponseDto>.NotFoundResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<StaffDocumentResponseDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document for staff {StaffId}", staffId);
            return StatusCode(500, ApiResponse<StaffDocumentResponseDto>.ErrorResponse("Internal server error", null, 500));
        }
    }

    /// <summary>Download a specific document.</summary>
    [HttpGet("{documentId}/download")]
    public async Task<IActionResult> DownloadDocument(int staffId, long documentId)
    {
        var companyId = GetCompanyId();
        if (companyId == null) return Unauthorized();

        try
        {
            var (stream, contentType, fileName) = await _service.DownloadDocumentAsync(documentId, staffId, companyId.Value);
            return File(stream, contentType, fileName);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading document {DocId} for staff {StaffId}", documentId, staffId);
            return StatusCode(500);
        }
    }

    /// <summary>Delete (soft-delete) a document.</summary>
    [HttpDelete("{documentId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteDocument(int staffId, long documentId)
    {
        var companyId = GetCompanyId();
        if (companyId == null)
            return Unauthorized(ApiResponse<object>.ErrorResponse("Unauthorized", null, 401));

        try
        {
            await _service.DeleteDocumentAsync(documentId, staffId, companyId.Value);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Document deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.NotFoundResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {DocId} for staff {StaffId}", documentId, staffId);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error", null, 500));
        }
    }
}
