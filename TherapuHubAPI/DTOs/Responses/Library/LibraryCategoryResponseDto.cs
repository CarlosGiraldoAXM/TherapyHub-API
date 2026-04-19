namespace TherapuHubAPI.DTOs.Responses.Library;

public class LibraryCategoryResponseDto
{
    public byte Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public bool IsActive { get; set; }
}
