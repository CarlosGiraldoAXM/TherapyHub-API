namespace TherapuHubAPI.DTOs.Requests.Auth;

public class SetInitialPasswordRequestDto
{
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
