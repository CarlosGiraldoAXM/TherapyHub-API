using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Responses.Staff;

public class StaffRoleResponseDto
{
    public short Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}
