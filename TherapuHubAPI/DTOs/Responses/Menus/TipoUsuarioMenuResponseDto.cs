namespace TherapuHubAPI.DTOs.Responses.Menus;

public class TipoUsuarioMenuResponseDto
{
    public int Id { get; set; }
    public int UserTypeId { get; set; }
    public string TipoUsuarioNombre { get; set; } = string.Empty;
    public int MenuId { get; set; }
    public string MenuTitulo { get; set; } = string.Empty;
    public string MenuRuta { get; set; } = string.Empty;
    public DateTime FechaAsignacion { get; set; }
}
