namespace TherapuHubAPI.DTOs.Responses.NoteCategories;

public class NoteCategoryResponseDto
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public int SectionId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
