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

    public async Task<IEnumerable<NoteResponseDto>> GetByOwnerAsync(int companyId, int menuId, int ownerActorId)
    {
        var notes = await _context.Notes
            .Where(n => n.CompanyId == companyId && n.MenuId == menuId && n.OwnerActorId == ownerActorId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        var priorities = await _context.NotePriorities.ToListAsync();
        return notes.Select(n => MapToDto(n, priorities));
    }

    public async Task<NoteResponseDto?> GetByIdAsync(long id, int companyId)
    {
        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == id && n.CompanyId == companyId);
        if (note == null) return null;

        var priorities = await _context.NotePriorities.ToListAsync();
        return MapToDto(note, priorities);
    }

    public async Task<NoteResponseDto> CreateAsync(CreateNoteRequestDto dto, int companyId, int createdByActorId)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new InvalidOperationException("Title is required.");

        // OwnerActorId: entity notes send their actorId explicitly; personal notes default to creator.
        int ownerActorId = (dto.OwnerActorId.HasValue && dto.OwnerActorId.Value != 0)
            ? dto.OwnerActorId.Value
            : createdByActorId;

        var note = new Notes
        {
            CompanyId = companyId,
            CreatedByActorId = createdByActorId,
            OwnerActorId = ownerActorId,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty,
            PriorityId = dto.PriorityId,
            NoteTypeId = dto.NoteTypeId,
            MenuId = dto.MenuId,
            SectionId = null,
            DueDate = dto.DueDate,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        var priorities = await _context.NotePriorities.ToListAsync();
        return MapToDto(note, priorities);
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
        return MapToDto(note, priorities);
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

    private static NoteResponseDto MapToDto(Notes note, IEnumerable<NotePriorities> priorities)
    {
        var priority = priorities.FirstOrDefault(p => p.Id == note.PriorityId);
        return new NoteResponseDto
        {
            Id = note.Id,
            CompanyId = note.CompanyId,
            CreatedByActorId = note.CreatedByActorId,
            OwnerActorId = note.OwnerActorId,
            Title = note.Title,
            Description = note.Description,
            PriorityId = note.PriorityId,
            PriorityName = priority?.Name,
            NoteTypeId = note.NoteTypeId,
            MenuId = note.MenuId,
            DueDate = note.DueDate,
            CreatedAt = note.CreatedAt,
            IsActive = note.IsActive,
        };
    }
}
