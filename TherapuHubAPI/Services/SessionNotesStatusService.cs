using TherapuHubAPI.DTOs.Requests.SessionNotesStatus;
using TherapuHubAPI.DTOs.Responses.SessionNotesStatus;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Services;

public class SessionNotesStatusService : ISessionNotesStatusService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SessionNotesStatusService> _logger;

    public SessionNotesStatusService(IUnitOfWork unitOfWork, ILogger<SessionNotesStatusService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<SessionNotesStatusResponseDto>> GetAllAsync()
    {
        var statuses = await _unitOfWork.SessionNotesStatuses.FindAsync(s => s.IsDelete != true);
        return statuses.Select(MapToDto);
    }

    public async Task<IEnumerable<SessionNotesStatusResponseDto>> GetActiveAsync()
    {
        var statuses = await _unitOfWork.SessionNotesStatuses.FindAsync(s => s.IsDelete != true && s.IsActive);
        return statuses.Select(MapToDto);
    }

    public async Task<SessionNotesStatusResponseDto?> GetByIdAsync(int id)
    {
        var status = await _unitOfWork.SessionNotesStatuses.GetByIdAsync(id);
        if (status == null || status.IsDelete == true) return null;
        return MapToDto(status);
    }

    public async Task<SessionNotesStatusResponseDto> CreateAsync(CreateSessionNotesStatusRequestDto dto, int actorId)
    {
        var entity = new SessionNotesStatus
        {
            Name = dto.Name.Trim(),
            Color = dto.Color,
            IsActive = dto.IsActive,
            ActorCreatedId = actorId,
            CreatedAt = DateTime.UtcNow,
        };
        await _unitOfWork.SessionNotesStatuses.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task<SessionNotesStatusResponseDto?> UpdateAsync(int id, UpdateSessionNotesStatusRequestDto dto)
    {
        var entity = await _unitOfWork.SessionNotesStatuses.GetByIdAsync(id);
        if (entity == null || entity.IsDelete == true) return null;

        entity.Name = dto.Name.Trim();
        entity.Color = dto.Color;
        entity.IsActive = dto.IsActive;
        _unitOfWork.SessionNotesStatuses.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id, int actorId)
    {
        var entity = await _unitOfWork.SessionNotesStatuses.GetByIdAsync(id);
        if (entity == null || entity.IsDelete == true) return false;

        entity.IsDelete = true;
        entity.DeleteAt = DateTime.UtcNow;
        entity.DeletedActorId = actorId;
        _unitOfWork.SessionNotesStatuses.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<SessionNotesStatusResponseDto?> ToggleActiveAsync(int id)
    {
        var entity = await _unitOfWork.SessionNotesStatuses.GetByIdAsync(id);
        if (entity == null || entity.IsDelete == true) return null;

        entity.IsActive = !entity.IsActive;
        _unitOfWork.SessionNotesStatuses.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(entity);
    }

    private static SessionNotesStatusResponseDto MapToDto(SessionNotesStatus s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Color = s.Color,
        IsActive = s.IsActive,
        CreatedAt = s.CreatedAt,
    };
}
