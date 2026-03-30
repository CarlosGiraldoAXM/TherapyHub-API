namespace TherapuHubAPI.DTOs.Requests.ActorRelationships;

public class AssignUserRequestDto
{
    public int SourceActorId { get; set; }
    public int TargetActorId { get; set; }
}
