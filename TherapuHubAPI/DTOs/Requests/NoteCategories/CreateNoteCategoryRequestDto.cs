namespace TherapuHubAPI.DTOs.Requests.NoteCategories;

public class CreateNoteCategoryRequestDto
{
    public string Name { get; set; } = null!;
    public int SectionId { get; set; }
}
