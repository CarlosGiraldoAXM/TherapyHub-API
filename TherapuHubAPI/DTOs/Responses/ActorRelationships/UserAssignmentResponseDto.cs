namespace TherapuHubAPI.DTOs.Responses.ActorRelationships;

public class UserAssignmentResponseDto
{
    public int UserId { get; set; }
    public int ActorId { get; set; }
    public string FullName { get; set; } = null!;
    public string? Email { get; set; }
    public string UserTypeName { get; set; } = null!;
    public bool IsAssigned { get; set; }
    public long? RelationshipId { get; set; }
}
