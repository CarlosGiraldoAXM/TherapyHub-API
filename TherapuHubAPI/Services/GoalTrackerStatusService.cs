using TherapuHubAPI.DTOs.Requests.GoalTrackerStatus;
using TherapuHubAPI.DTOs.Responses.GoalTrackerStatus;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Services;

public class GoalTrackerStatusService : IGoalTrackerStatusService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GoalTrackerStatusService> _logger;

    public GoalTrackerStatusService(IUnitOfWork unitOfWork, ILogger<GoalTrackerStatusService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<GoalTrackerStatusResponseDto>> GetAllAsync()
    {
        var statuses = await _unitOfWork.GoalTrackerStatuses.FindAsync(s => s.IsDelete != true);
        return statuses.Select(MapToDto);
    }

    public async Task<IEnumerable<GoalTrackerStatusResponseDto>> GetActiveAsync()
    {
        var statuses = await _unitOfWork.GoalTrackerStatuses.FindAsync(s => s.IsDelete != true && s.IsActive);
        return statuses.Select(MapToDto);
    }

    public async Task<GoalTrackerStatusResponseDto?> GetByIdAsync(int id)
    {
        var status = await _unitOfWork.GoalTrackerStatuses.GetByIdAsync(id);
        if (status == null || status.IsDelete == true) return null;
        return MapToDto(status);
    }

    public async Task<GoalTrackerStatusResponseDto> CreateAsync(CreateGoalTrackerStatusRequestDto dto, int actorId)
    {
        var entity = new GoalTrackerStatus
        {
            Name = dto.Name.Trim(),
            Color = dto.Color,
            IsActive = dto.IsActive,
            ActorCreatedId = actorId,
            CreatedAt = DateTime.UtcNow,
        };
        await _unitOfWork.GoalTrackerStatuses.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task<GoalTrackerStatusResponseDto?> UpdateAsync(int id, UpdateGoalTrackerStatusRequestDto dto)
    {
        var entity = await _unitOfWork.GoalTrackerStatuses.GetByIdAsync(id);
        if (entity == null || entity.IsDelete == true) return null;

        entity.Name = dto.Name.Trim();
        entity.Color = dto.Color;
        entity.IsActive = dto.IsActive;
        _unitOfWork.GoalTrackerStatuses.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id, int actorId)
    {
        var entity = await _unitOfWork.GoalTrackerStatuses.GetByIdAsync(id);
        if (entity == null || entity.IsDelete == true) return false;

        entity.IsDelete = true;
        entity.DeleteAt = DateTime.UtcNow;
        entity.DeletedActorId = actorId;
        _unitOfWork.GoalTrackerStatuses.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<GoalTrackerStatusResponseDto?> ToggleActiveAsync(int id)
    {
        var entity = await _unitOfWork.GoalTrackerStatuses.GetByIdAsync(id);
        if (entity == null || entity.IsDelete == true) return null;

        entity.IsActive = !entity.IsActive;
        _unitOfWork.GoalTrackerStatuses.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(entity);
    }

    private static GoalTrackerStatusResponseDto MapToDto(GoalTrackerStatus s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Color = s.Color,
        IsActive = s.IsActive,
        CreatedAt = s.CreatedAt,
    };
}
