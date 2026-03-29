namespace TherapuHubAPI.DTOs.Requests.GoalTrackerStatus;

public class UpdateGoalTrackerStatusRequestDto
{
    public string Name { get; set; } = null!;
    public string Color { get; set; } = null!;
    public bool IsActive { get; set; }
}
