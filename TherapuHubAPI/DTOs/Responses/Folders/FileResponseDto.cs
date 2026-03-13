namespace TherapuHubAPI.DTOs.Responses.Folders;

public class FileResponseDto
{
    public long Id { get; set; }
    public int FolderId { get; set; }
    public string FileName { get; set; } = null!;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = null!;
    public int UploadedByUserId { get; set; }
    public DateTime UploadedAt { get; set; }
}
