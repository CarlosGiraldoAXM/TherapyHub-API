namespace TherapuHubAPI.DTOs.Responses.Files;

public class EntityFileResponseDto
{
    public long Id { get; set; }
    public string FileName { get; set; } = null!;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = null!;
    public int UploadedByActorId { get; set; }
    public DateTime UploadedAt { get; set; }
    public int OwnerActorId { get; set; }
    public int FilesTypeId { get; set; }
}
