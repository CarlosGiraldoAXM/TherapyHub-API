namespace TherapuHubAPI.DTOs.Responses.Library;

public class LibraryItemFileResponseDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
}
