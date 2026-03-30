using TherapuHubAPI.DTOs.Requests.ActorRelationships;
using TherapuHubAPI.DTOs.Responses.ActorRelationships;

namespace TherapuHubAPI.Services.IServices;

public interface IActorRelationshipService
{
    // Supervisor → RBT (type 1)
    Task<IEnumerable<SupervisorResponseDto>> GetSupervisorsAsync(int companyId);
    Task<IEnumerable<RbtAssignmentResponseDto>> GetRbtsForSupervisorAsync(int supervisorActorId, int companyId);
    Task<ActorRelationshipResponseDto> AssignRbtAsync(AssignRbtRequestDto request, int companyId);

    // User → User (type 2)
    Task<IEnumerable<SupervisorResponseDto>> GetUserSourcesAsync(int companyId);
    Task<IEnumerable<UserAssignmentResponseDto>> GetUsersForSourceAsync(int sourceActorId, int companyId);
    Task<ActorRelationshipResponseDto> AssignUserAsync(AssignUserRequestDto request, int companyId);

    // Shared
    Task<bool> RemoveAssignmentAsync(long relationshipId, int companyId);
}
