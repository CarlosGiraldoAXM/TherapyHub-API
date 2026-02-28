namespace TherapuHubAPI.DTOs.Requests.Events;

public class UpdateEventTypeRequestDto
{
    public string Name { get; set; } = null!;
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
}
