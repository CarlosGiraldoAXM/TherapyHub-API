using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.DTOs.Requests.Notes;
using TherapuHubAPI.DTOs.Responses.Notes;
using TherapuHubAPI.Models;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Services;

public class NotesService : INotesService
{
    private readonly ContextDB _context;
    private readonly ILogger<NotesService> _logger;

    public NotesService(ContextDB context, ILogger<NotesService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Finds the NoteSection for (moduleId, entityId). Returns null if not found or entityId is null/0.
    /// </summary>
    private async Task<NoteSections?> FindSectionAsync(int moduleId, int entityId)
    {
        var name = entityId.ToString();
        return await _context.NoteSections
            .FirstOrDefaultAsync(s => s.ModuleId == moduleId && s.Name == name);
    }

    /// <summary>
    /// Finds or auto-creates the NoteSection for (moduleId, entityId).
    /// </summary>
    private async Task<NoteSections> GetOrCreateSectionAsync(int moduleId, int entityId)
    {
        var name = entityId.ToString();
        var section = await _context.NoteSections
            .FirstOrDefaultAsync(s => s.ModuleId == moduleId && s.Name == name);

        if (section == null)
        {
            section = new NoteSections
            {
                ModuleId = moduleId,
                Name = name,
                IsActive = true,
            };
            _context.NoteSections.Add(section);
            await _context.SaveChangesAsync();
        }

        return section;
    }

    public async Task<IEnumerable<NoteResponseDto>> GetByModuleEntityAsync(int companyId, int moduleId, int? entityId)
    {
        IQueryable<Notes> query = _context.Notes
            .Where(n => n.CompanyId == companyId && n.ModuleId == moduleId);

        if (entityId.HasValue && entityId.Value != 0)
        {
            // Notes scoped to a specific entity (e.g. a Staff member)
            var section = await FindSectionAsync(moduleId, entityId.Value);
            if (section == null) return Enumerable.Empty<NoteResponseDto>();
            query = query.Where(n => n.SectionId == section.Id);
        }
        else
        {
            // Module-level notes with no section (e.g. Todo page)
            query = query.Where(n => n.SectionId == null);
        }

        var notes = await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
        var priorities = await _context.NotePriorities.ToListAsync();
        return notes.Select(n => MapToDto(n, priorities, entityId ?? 0));
    }

    public async Task<NoteResponseDto?> GetByIdAsync(long id, int companyId)
    {
        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == id && n.CompanyId == companyId);
        if (note == null) return null;

        var priorities = await _context.NotePriorities.ToListAsync();

        var entityId = 0;
        if (note.SectionId.HasValue)
        {
            var section = await _context.NoteSections.FindAsync(note.SectionId.Value);
            if (section != null) int.TryParse(section.Name, out entityId);
        }

        return MapToDto(note, priorities, entityId);
    }

    public async Task<NoteResponseDto> CreateAsync(CreateNoteRequestDto dto, int companyId, int createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new InvalidOperationException("Title is required.");

        int? sectionId = null;
        if (dto.EntityId.HasValue && dto.EntityId.Value != 0)
        {
            var section = await GetOrCreateSectionAsync(dto.ModuleId, dto.EntityId.Value);
            sectionId = section.Id;
        }

        var note = new Notes
        {
            CompanyId = companyId,
            CreatedByUserId = createdByUserId,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty,
            PriorityId = dto.PriorityId,
            NoteTypeId = dto.NoteTypeId,
            ModuleId = dto.ModuleId,
            SectionId = sectionId,
            DueDate = dto.DueDate,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        var priorities = await _context.NotePriorities.ToListAsync();
        return MapToDto(note, priorities, dto.EntityId ?? 0);
    }

    public async Task<NoteResponseDto?> UpdateAsync(long id, UpdateNoteRequestDto dto, int companyId)
    {
        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == id && n.CompanyId == companyId);
        if (note == null) return null;

        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new InvalidOperationException("Title is required.");

        note.Title = dto.Title.Trim();
        note.Description = dto.Description?.Trim() ?? string.Empty;
        note.PriorityId = dto.PriorityId;
        note.DueDate = dto.DueDate;
        note.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        var priorities = await _context.NotePriorities.ToListAsync();

        var entityId = 0;
        if (note.SectionId.HasValue)
        {
            var section = await _context.NoteSections.FindAsync(note.SectionId.Value);
            if (section != null) int.TryParse(section.Name, out entityId);
        }

        return MapToDto(note, priorities, entityId);
    }

    public async Task<bool> DeleteAsync(long id, int companyId)
    {
        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == id && n.CompanyId == companyId);
        if (note == null) return false;

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleActiveAsync(long id, int companyId)
    {
        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == id && n.CompanyId == companyId);
        if (note == null) return false;

        note.IsActive = !note.IsActive;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<NotePriorityResponseDto>> GetPrioritiesAsync()
    {
        return await _context.NotePriorities
            .Where(p => p.IsActive)
            .OrderBy(p => p.Id)
            .Select(p => new NotePriorityResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                IsActive = p.IsActive,
            })
            .ToListAsync();
    }

    private static NoteResponseDto MapToDto(Notes note, IEnumerable<NotePriorities> priorities, int entityId)
    {
        var priority = priorities.FirstOrDefault(p => p.Id == note.PriorityId);
        return new NoteResponseDto
        {
            Id = note.Id,
            CompanyId = note.CompanyId,
            CreatedByUserId = note.CreatedByUserId,
            Title = note.Title,
            Description = note.Description,
            PriorityId = note.PriorityId,
            PriorityName = priority?.Name,
            NoteTypeId = note.NoteTypeId,
            ModuleId = note.ModuleId,
            SectionId = note.SectionId,
            EntityId = entityId,
            DueDate = note.DueDate,
            CreatedAt = note.CreatedAt,
            IsActive = note.IsActive,
        };
    }
}
