namespace TherapuHubAPI.DTOs.Responses.ActorRelationships;

public class ActorRelationshipResponseDto
{
    public long Id { get; set; }
    public int SupervisorActorId { get; set; }
    public int RbtActorId { get; set; }
    public string RbtName { get; set; } = null!;
    public int RbtStaffId { get; set; }
    public DateTime CreatedAt { get; set; }
}
