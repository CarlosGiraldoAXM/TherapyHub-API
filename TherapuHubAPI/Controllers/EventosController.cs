using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.Events;
using TherapuHubAPI.DTOs.Responses.Events;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly IEventosService _eventosService;

    public EventosController(IEventosService eventosService)
    {
        _eventosService = eventosService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EventoResponseDto>>>> GetEventos(
        [FromQuery] DateTime? start, 
        [FromQuery] DateTime? end,
        [FromQuery] bool? esTodoElDia)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        
        int userId = int.Parse(userIdClaim.Value);
        var eventos = await _eventosService.GetEventosByUserAsync(userId, start, end, esTodoElDia);
        
        return Ok(new ApiResponse<IEnumerable<EventoResponseDto>>
        {
            Success = true,
            Data = eventos,
            Message = "Events obtenidos correctamente"
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<EventoResponseDto>>> GetEvento(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        int currentUserId = int.Parse(userIdClaim.Value);
        var evento = await _eventosService.GetEventoByIdAsync(id, currentUserId);
        if (evento == null)
        {
            return NotFound(new ApiResponse<EventoResponseDto>
            {
                Success = false,
                Message = "Event not found or access denied."
            });
        }

        return Ok(new ApiResponse<EventoResponseDto>
        {
            Success = true,
            Data = evento
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<EventoResponseDto>>> CreateEvento(CreateEventoRequestDto request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        int currentUserId = int.Parse(userIdClaim.Value);
        var result = await _eventosService.CreateEventoAsync(request, currentUserId);
        return CreatedAtAction(nameof(GetEvento), new { id = result.IdEvento }, new ApiResponse<EventoResponseDto>
        {
            Success = true,
            Data = result,
            Message = "Evento creado correctamente"
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateEvento(int id, UpdateEventoRequestDto request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        int currentUserId = int.Parse(userIdClaim.Value);
        var success = await _eventosService.UpdateEventoAsync(id, request, currentUserId);
        if (!success)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Event not found or update not allowed."
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Event updated successfully."
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteEvento(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        int currentUserId = int.Parse(userIdClaim.Value);
        var success = await _eventosService.DeleteEventoAsync(id, currentUserId);
        if (!success)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Event not found or delete not allowed."
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Event deleted successfully."
        });
    }
}
