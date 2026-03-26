namespace TherapuHubAPI.DTOs.Responses.Folders;

public class FolderResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public byte FolderTypeId { get; set; }
    public string? FolderTypeName { get; set; }
    public int CompanyId { get; set; }
    public int? CreatedByActorId { get; set; }
    public int? OwnerActorId { get; set; }
    public int? MenuId { get; set; }
    public int? ParentFolderId { get; set; }
    public string Path { get; set; } = null!;
    public int Level { get; set; }
    public bool IsGlobal { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public int FileCount { get; set; }
}
