namespace TherapuHubAPI.DTOs.Requests.GoalTrackerStatus;

public class CreateGoalTrackerStatusRequestDto
{
    public string Name { get; set; } = null!;
    public string Color { get; set; } = "#6366f1";
    public bool IsActive { get; set; } = true;
}
