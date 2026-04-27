namespace TherapuHubAPI.DTOs.Responses.ActorRelationships;

public class ClientAssignmentResponseDto
{
    public int ClientId { get; set; }
    public int ActorId { get; set; }
    public string FullName { get; set; } = null!;
    public bool IsAssigned { get; set; }
    public long? RelationshipId { get; set; }
}
