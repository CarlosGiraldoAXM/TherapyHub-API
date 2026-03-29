using System.ComponentModel.DataAnnotations;

namespace TherapuHubAPI.DTOs.Requests.GoalTracker;

public class CreateGoalTrackerRowRequestDto
{
    [Required]
    public int OwnerActorId { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateGoalTrackerItemDto> Items { get; set; } = [];
}
