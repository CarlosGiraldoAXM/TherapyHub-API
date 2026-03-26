namespace TherapuHubAPI.DTOs.Requests.Folders;

public class CreateFolderRequestDto
{
    public string Name { get; set; } = null!;
    public byte FolderTypeId { get; set; }
    public int? ParentFolderId { get; set; }
    public bool IsGlobal { get; set; } = true;
    public int? OwnerActorId { get; set; }
    public int? MenuId { get; set; }
}
