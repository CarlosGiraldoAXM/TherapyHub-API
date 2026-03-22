namespace TherapuHubAPI.DTOs.Responses.Staff;

public class StaffDocumentResponseDto
{
    public long Id { get; set; }
    public int StaffId { get; set; }
    public byte DocumentTypeId { get; set; }
    public string DocumentTypeName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}
