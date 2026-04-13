using TherapuHubAPI.DTOs.Requests.GoalTrackerStatus;
using TherapuHubAPI.DTOs.Responses.GoalTrackerStatus;

namespace TherapuHubAPI.Services.IServices;

public interface IGoalTrackerStatusService
{
    Task<IEnumerable<GoalTrackerStatusResponseDto>> GetAllAsync();
    Task<IEnumerable<GoalTrackerStatusResponseDto>> GetActiveAsync();
    Task<GoalTrackerStatusResponseDto?> GetByIdAsync(int id);
    Task<GoalTrackerStatusResponseDto> CreateAsync(CreateGoalTrackerStatusRequestDto dto, int actorId);
    Task<GoalTrackerStatusResponseDto?> UpdateAsync(int id, UpdateGoalTrackerStatusRequestDto dto);
    Task<bool> DeleteAsync(int id, int actorId);
    Task<GoalTrackerStatusResponseDto?> ToggleActiveAsync(int id);
    Task<IEnumerable<GoalTrackerStatusResponseDto>?> MoveAsync(int id, string direction);
}
