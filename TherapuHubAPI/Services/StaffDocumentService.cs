using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.DTOs.Responses.Staff;
using TherapuHubAPI.Models;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Services;

public class StaffDocumentService : IStaffDocumentService
{
    private readonly ContextDB _context;
    private readonly IFileStorageService _storage;
    private readonly ILogger<StaffDocumentService> _logger;

    public StaffDocumentService(ContextDB context, IFileStorageService storage, ILogger<StaffDocumentService> logger)
    {
        _context = context;
        _storage = storage;
        _logger = logger;
    }

    public async Task<IEnumerable<StaffDocumentResponseDto>> GetDocumentsByStaffAsync(int staffId, int companyId)
    {
        // Verify staff belongs to the company
        var staffExists = await _context.Staff
            .AnyAsync(s => s.Id == staffId && s.CompanyId == companyId && s.IsActive);

        if (!staffExists)
            throw new KeyNotFoundException($"Staff {staffId} not found.");

        var docs = await _context.StaffDocuments
            .Where(d => d.StaffId == staffId && d.IsActive)
            .OrderByDescending(d => d.UploadedAt)
            .Join(_context.StaffDocumentTypes,
                d => d.DocumentTypeId,
                t => t.Id,
                (d, t) => new StaffDocumentResponseDto
                {
                    Id = d.Id,
                    StaffId = d.StaffId,
                    DocumentTypeId = d.DocumentTypeId,
                    DocumentTypeName = t.Name,
                    FileName = d.FileName,
                    UploadedAt = d.UploadedAt,
                })
            .ToListAsync();

        return docs;
    }

    public async Task<StaffDocumentResponseDto> UploadDocumentAsync(int staffId, int companyId, IFormFile file, byte documentTypeId)
    {
        // Verify staff belongs to the company
        var staffExists = await _context.Staff
            .AnyAsync(s => s.Id == staffId && s.CompanyId == companyId && s.IsActive);

        if (!staffExists)
            throw new KeyNotFoundException($"Staff {staffId} not found.");

        // Enforce PDF only
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext != ".pdf" || file.ContentType != "application/pdf")
            throw new InvalidOperationException("Only PDF files are allowed.");

        // Upload to blob storage
        var containerPath = $"staff/{staffId}/documents";
        var blobPath = await _storage.SaveFileAsync(file, containerPath);

        // Deactivate any previous document of the same type
        var existing = await _context.StaffDocuments
            .Where(d => d.StaffId == staffId && d.DocumentTypeId == documentTypeId && d.IsActive)
            .ToListAsync();

        foreach (var old in existing)
        {
            old.IsActive = false;
            try { await _storage.DeleteFileAsync(old.BlobUrl); }
            catch (Exception ex) { _logger.LogWarning(ex, "Could not delete old blob {Blob}", old.BlobUrl); }
        }

        // Persist new record
        var doc = new StaffDocuments
        {
            StaffId = staffId,
            DocumentTypeId = documentTypeId,
            BlobUrl = blobPath,
            FileName = file.FileName,
            UploadedAt = DateTime.UtcNow,
            IsActive = true,
        };

        _context.StaffDocuments.Add(doc);
        await _context.SaveChangesAsync();

        var typeName = await _context.StaffDocumentTypes
            .Where(t => t.Id == documentTypeId)
            .Select(t => t.Name)
            .FirstOrDefaultAsync() ?? string.Empty;

        return new StaffDocumentResponseDto
        {
            Id = doc.Id,
            StaffId = doc.StaffId,
            DocumentTypeId = doc.DocumentTypeId,
            DocumentTypeName = typeName,
            FileName = doc.FileName,
            UploadedAt = doc.UploadedAt,
        };
    }

    public async Task<(Stream Stream, string ContentType, string FileName)> DownloadDocumentAsync(long documentId, int staffId, int companyId)
    {
        var staffExists = await _context.Staff
            .AnyAsync(s => s.Id == staffId && s.CompanyId == companyId && s.IsActive);

        if (!staffExists)
            throw new KeyNotFoundException($"Staff {staffId} not found.");

        var doc = await _context.StaffDocuments
            .FirstOrDefaultAsync(d => d.Id == documentId && d.StaffId == staffId && d.IsActive);

        if (doc == null)
            throw new KeyNotFoundException($"Document {documentId} not found.");

        return await _storage.GetFileAsync(doc.BlobUrl);
    }

    public async Task DeleteDocumentAsync(long documentId, int staffId, int companyId)
    {
        var staffExists = await _context.Staff
            .AnyAsync(s => s.Id == staffId && s.CompanyId == companyId && s.IsActive);

        if (!staffExists)
            throw new KeyNotFoundException($"Staff {staffId} not found.");

        var doc = await _context.StaffDocuments
            .FirstOrDefaultAsync(d => d.Id == documentId && d.StaffId == staffId && d.IsActive);

        if (doc == null)
            throw new KeyNotFoundException($"Document {documentId} not found.");

        doc.IsActive = false;
        await _context.SaveChangesAsync();

        try { await _storage.DeleteFileAsync(doc.BlobUrl); }
        catch (Exception ex) { _logger.LogWarning(ex, "Could not delete blob {Blob}", doc.BlobUrl); }
    }
}
