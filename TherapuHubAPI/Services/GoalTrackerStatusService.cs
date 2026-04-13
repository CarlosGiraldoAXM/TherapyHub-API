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
        return statuses.OrderBy(s => s.SortOrder ?? int.MaxValue).ThenBy(s => s.Id).Select(MapToDto);
    }

    public async Task<IEnumerable<GoalTrackerStatusResponseDto>> GetActiveAsync()
    {
        var statuses = await _unitOfWork.GoalTrackerStatuses.FindAsync(s => s.IsDelete != true && s.IsActive);
        return statuses.OrderBy(s => s.SortOrder ?? int.MaxValue).ThenBy(s => s.Id).Select(MapToDto);
    }

    public async Task<GoalTrackerStatusResponseDto?> GetByIdAsync(int id)
    {
        var status = await _unitOfWork.GoalTrackerStatuses.GetByIdAsync(id);
        if (status == null || status.IsDelete == true) return null;
        return MapToDto(status);
    }

    public async Task<GoalTrackerStatusResponseDto> CreateAsync(CreateGoalTrackerStatusRequestDto dto, int actorId)
    {
        var all = await _unitOfWork.GoalTrackerStatuses.FindAsync(s => s.IsDelete != true);
        var maxSort = all.Any() ? all.Max(s => s.SortOrder ?? 0) : 0;

        var entity = new GoalTrackerStatus
        {
            Name = dto.Name.Trim(),
            Color = dto.Color,
            IsActive = dto.IsActive,
            ActorCreatedId = actorId,
            CreatedAt = DateTime.UtcNow,
            SortOrder = maxSort + 1,
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

    public async Task<IEnumerable<GoalTrackerStatusResponseDto>?> MoveAsync(int id, string direction)
    {
        var all = (await _unitOfWork.GoalTrackerStatuses.FindAsync(s => s.IsDelete != true))
            .OrderBy(s => s.SortOrder ?? int.MaxValue).ThenBy(s => s.Id).ToList();

        var index = all.FindIndex(s => s.Id == id);
        if (index < 0) return null;

        int swapIndex = direction == "up" ? index - 1 : index + 1;
        if (swapIndex < 0 || swapIndex >= all.Count) return ImmutableResults(all);

        var target = all[index];
        var neighbor = all[swapIndex];

        // Ensure both have numeric SortOrder before swapping
        if (target.SortOrder == null || neighbor.SortOrder == null)
        {
            // Assign sequential SortOrder to all first
            for (int i = 0; i < all.Count; i++) all[i].SortOrder = i + 1;
            _unitOfWork.GoalTrackerStatuses.Update(all[index]);
            _unitOfWork.GoalTrackerStatuses.Update(all[swapIndex]);
        }

        (target.SortOrder, neighbor.SortOrder) = (neighbor.SortOrder, target.SortOrder);
        _unitOfWork.GoalTrackerStatuses.Update(target);
        _unitOfWork.GoalTrackerStatuses.Update(neighbor);
        await _unitOfWork.SaveChangesAsync();

        all = (await _unitOfWork.GoalTrackerStatuses.FindAsync(s => s.IsDelete != true))
            .OrderBy(s => s.SortOrder ?? int.MaxValue).ThenBy(s => s.Id).ToList();
        return all.Select(MapToDto);
    }

    private static IEnumerable<GoalTrackerStatusResponseDto> ImmutableResults(List<GoalTrackerStatus> list)
        => list.Select(MapToDto);

    private static GoalTrackerStatusResponseDto MapToDto(GoalTrackerStatus s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Color = s.Color,
        IsActive = s.IsActive,
        SortOrder = s.SortOrder,
        CreatedAt = s.CreatedAt,
    };
}
