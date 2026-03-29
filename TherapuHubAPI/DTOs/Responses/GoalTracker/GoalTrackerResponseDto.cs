namespace TherapuHubAPI.DTOs.Responses.GoalTracker;

public class GoalTrackerResponseDto
{
    public long TrackerId { get; set; }
    public int OwnerActorId { get; set; }
    public IEnumerable<GoalTrackerItemResponseDto> Items { get; set; } = [];
}
