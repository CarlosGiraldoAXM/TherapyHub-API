namespace TherapuHubAPI.DTOs.Requests.NoteCategories;

public class UpdateNoteCategoryRequestDto
{
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}
