using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Requests.Staff;

public class CreateStaffRequestDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public short RoleId { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string Phone { get; set; } = null!;
    public string? Certifications { get; set; }
    public string Email { get; set; } = null!;
    public byte StatusId { get; set; }
    public DateOnly? ContractDate { get; set; }
}
