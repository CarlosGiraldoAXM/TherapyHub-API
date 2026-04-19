namespace TherapuHubAPI.DTOs.Requests.Library;

public class UpsertLibraryPermissionRequestDto
{
    public int ActorId { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}
