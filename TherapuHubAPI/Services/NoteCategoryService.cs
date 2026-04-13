using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.DTOs.Requests.NoteCategories;
using TherapuHubAPI.DTOs.Responses.NoteCategories;
using TherapuHubAPI.Models;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Services;

public class NoteCategoryService : INoteCategoryService
{
    private readonly ContextDB _context;
    private readonly ILogger<NoteCategoryService> _logger;

    public NoteCategoryService(ContextDB context, ILogger<NoteCategoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<NoteCategoryResponseDto>> GetBySectionAsync(int companyId, int sectionId)
    {
        return await _context.NoteCategories
            .Where(c => c.CompanyId == companyId && c.SectionId == sectionId)
            .OrderBy(c => c.Name)
            .Select(c => MapToDto(c))
            .ToListAsync();
    }

    public async Task<NoteCategoryResponseDto?> GetByIdAsync(int id, int companyId)
    {
        var entity = await _context.NoteCategories
            .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == companyId);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<NoteCategoryResponseDto> CreateAsync(CreateNoteCategoryRequestDto dto, int companyId, int actorId)
    {
        var name = dto.Name.Trim();
        var createdAt = DateTime.UtcNow;

        // HasNoKey workaround: use raw SQL for the INSERT
        await _context.Database.ExecuteSqlInterpolatedAsync(
            $"""
            INSERT INTO NoteCategories (CompanyId, CreatedByActorId, SectionId, Name, IsActive, CreatedAt)
            VALUES ({companyId}, {actorId}, {dto.SectionId}, {name}, 1, {createdAt})
            """);

        // Re-query to get the generated Id
        var created = await _context.NoteCategories
            .Where(c => c.CompanyId == companyId && c.SectionId == dto.SectionId
                     && c.CreatedByActorId == actorId && c.Name == name)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();

        return MapToDto(created!);
    }

    public async Task<NoteCategoryResponseDto?> UpdateAsync(int id, UpdateNoteCategoryRequestDto dto, int companyId)
    {
        var exists = await _context.NoteCategories
            .AnyAsync(c => c.Id == id && c.CompanyId == companyId);
        if (!exists) return null;

        await _context.NoteCategories
            .Where(c => c.Id == id && c.CompanyId == companyId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.Name, dto.Name.Trim())
                .SetProperty(c => c.IsActive, dto.IsActive));

        return await GetByIdAsync(id, companyId);
    }

    public async Task<bool> DeleteAsync(int id, int companyId)
    {
        var deleted = await _context.NoteCategories
            .Where(c => c.Id == id && c.CompanyId == companyId)
            .ExecuteDeleteAsync();
        return deleted > 0;
    }

    public async Task<NoteCategoryResponseDto?> ToggleActiveAsync(int id, int companyId)
    {
        var entity = await _context.NoteCategories
            .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == companyId);
        if (entity is null) return null;

        await _context.NoteCategories
            .Where(c => c.Id == id && c.CompanyId == companyId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsActive, !entity.IsActive));

        return await GetByIdAsync(id, companyId);
    }

    private static NoteCategoryResponseDto MapToDto(NoteCategories c) => new()
    {
        Id = c.Id,
        CompanyId = c.CompanyId,
        SectionId = c.SectionId,
        Name = c.Name,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt,
    };
}
