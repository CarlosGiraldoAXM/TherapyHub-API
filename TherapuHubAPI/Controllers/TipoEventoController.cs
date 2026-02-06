using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Responses.Events;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TipoEventoController : ControllerBase
{
    private readonly ITipoEventoService _tipoEventoService;

    public TipoEventoController(ITipoEventoService tipoEventoService)
    {
        _tipoEventoService = tipoEventoService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<TipoEventoResponseDto>>>> GetTiposEvento()
    {
        var tipos = await _tipoEventoService.GetTiposEventoActivosAsync();
        return Ok(new ApiResponse<IEnumerable<TipoEventoResponseDto>>
        {
            Success = true,
            Data = tipos,
            Message = "Tipos de evento obtenidos correctamente"
        });
    }
}
