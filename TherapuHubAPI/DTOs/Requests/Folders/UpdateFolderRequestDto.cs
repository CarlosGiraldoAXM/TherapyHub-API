namespace TherapuHubAPI.DTOs.Requests.Folders;

public class UpdateFolderRequestDto
{
    public string Name { get; set; } = null!;
    public bool IsGlobal { get; set; }
}
