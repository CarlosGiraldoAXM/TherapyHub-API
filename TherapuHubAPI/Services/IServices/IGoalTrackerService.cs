using TherapuHubAPI.DTOs.Requests.GoalTracker;
using TherapuHubAPI.DTOs.Responses.GoalTracker;

namespace TherapuHubAPI.Services.IServices;

public interface IGoalTrackerService
{
    Task<IEnumerable<GoalTrackerCategoryResponseDto>> GetCategoriesAsync();
    Task<GoalTrackerResponseDto?> GetByOwnerAsync(int ownerActorId);
    Task<GoalTrackerResponseDto> CreateRowAsync(CreateGoalTrackerRowRequestDto dto, int actorId);
    Task<GoalTrackerItemResponseDto?> UpdateItemAsync(long itemId, UpdateGoalTrackerItemRequestDto dto);
    Task<bool> DeleteItemAsync(long itemId);
}
