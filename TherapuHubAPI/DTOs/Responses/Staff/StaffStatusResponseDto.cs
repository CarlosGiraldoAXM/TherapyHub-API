using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Responses.Staff;

public class StaffStatusResponseDto
{
    public byte Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}
