namespace TherapuHubAPI.DTOs.Responses.ActorRelationships;

public class SupervisorResponseDto
{
    public int UserId { get; set; }
    public int ActorId { get; set; }
    public string FullName { get; set; } = null!;
    public string? Email { get; set; }
    public string UserTypeName { get; set; } = null!;
    public int AssignedRbtCount { get; set; }
}
