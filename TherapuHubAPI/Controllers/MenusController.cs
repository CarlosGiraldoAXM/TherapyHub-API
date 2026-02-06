using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.Menus;
using TherapuHubAPI.DTOs.Responses.Menus;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;
    private readonly ILogger<MenusController> _logger;

    public MenusController(
        IMenuService menuService,
        ILogger<MenusController> logger)
    {
        _menuService = menuService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los menús disponibles
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MenuResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MenuResponseDto>>>> GetAll()
    {
        try
        {
            var result = await _menuService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<MenuResponseDto>>.SuccessResponse(
                result,
                "Menus retrieved successfully",
                200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving menus");
            return StatusCode(500, ApiResponse<IEnumerable<MenuResponseDto>>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Obtiene los menús asignados a un tipo de usuario
    /// </summary>
    [HttpGet("tipo-usuario/{tipoUsuarioId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MenuResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MenuResponseDto>>>> GetMenusByTipoUsuarioId(int tipoUsuarioId)
    {
        try
        {
            var result = await _menuService.GetMenusByTipoUsuarioIdAsync(tipoUsuarioId);
            return Ok(ApiResponse<IEnumerable<MenuResponseDto>>.SuccessResponse(
                result,
                "Menus retrieved successfully",
                200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving menus for user type Id: {UserTypeId}", tipoUsuarioId);
            return StatusCode(500, ApiResponse<IEnumerable<MenuResponseDto>>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Obtiene un tipo de usuario con sus menús asignados
    /// </summary>
    [HttpGet("tipo-usuario/{tipoUsuarioId}/completo")]
    [ProducesResponseType(typeof(ApiResponse<TipoUsuarioConMenusResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<TipoUsuarioConMenusResponseDto>>> GetTipoUsuarioConMenus(int tipoUsuarioId)
    {
        try
        {
            var result = await _menuService.GetTipoUsuarioConMenusAsync(tipoUsuarioId);
            return Ok(ApiResponse<TipoUsuarioConMenusResponseDto>.SuccessResponse(
                result,
                "User type with menus retrieved successfully",
                200));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User type not found: {UserTypeId}", tipoUsuarioId);
            return NotFound(ApiResponse<TipoUsuarioConMenusResponseDto>.NotFoundResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user type with menus: {UserTypeId}", tipoUsuarioId);
            return StatusCode(500, ApiResponse<TipoUsuarioConMenusResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Asigna menús a un tipo de usuario
    /// </summary>
    [HttpPost("asignar")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object>>> AsignarMenus([FromBody] AsignarMenusRequestDto request)
    {
        try
        {
            await _menuService.AsignarMenusAsync(request);
            return Ok(ApiResponse<object>.SuccessResponse(
                new { },
                "Menus assigned successfully",
                200));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error assigning menus");
            return BadRequest(ApiResponse<object>.ErrorResponse(
                ex.Message,
                new List<string> { ex.Message },
                400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning menus");
            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Obtiene un menú por Id
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MenuResponseDto>>> GetById(int id)
    {
        try
        {
            var result = await _menuService.GetByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponse<MenuResponseDto>.NotFoundResponse($"Menu with Id {id} not found."));
            return Ok(ApiResponse<MenuResponseDto>.SuccessResponse(result, "Menu retrieved successfully", 200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving menu Id: {Id}", id);
            return StatusCode(500, ApiResponse<MenuResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Crea un nuevo menú (contenedor o submenú)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<MenuResponseDto>>> Create([FromBody] CreateMenuRequestDto request)
    {
        try
        {
            var result = await _menuService.CreateAsync(request);
            return Ok(ApiResponse<MenuResponseDto>.SuccessResponse(result, "Menu created successfully", 200));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error creating menu");
            return BadRequest(ApiResponse<MenuResponseDto>.ErrorResponse(ex.Message, new List<string> { ex.Message }, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating menu");
            return StatusCode(500, ApiResponse<MenuResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Actualiza un menú existente
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MenuResponseDto>>> Update(int id, [FromBody] UpdateMenuRequestDto request)
    {
        try
        {
            var result = await _menuService.UpdateAsync(id, request);
            return Ok(ApiResponse<MenuResponseDto>.SuccessResponse(result, "Menu updated successfully", 200));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error updating menu Id: {Id}", id);
            if (ex.Message.Contains("not found"))
                return NotFound(ApiResponse<MenuResponseDto>.NotFoundResponse(ex.Message));
            return BadRequest(ApiResponse<MenuResponseDto>.ErrorResponse(ex.Message, new List<string> { ex.Message }, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating menu Id: {Id}", id);
            return StatusCode(500, ApiResponse<MenuResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Elimina un menú (no puede tener submenús)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            await _menuService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Menu deleted successfully", 200));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error deleting menu Id: {Id}", id);
            if (ex.Message.Contains("not found"))
                return NotFound(ApiResponse<object>.NotFoundResponse(ex.Message));
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, new List<string> { ex.Message }, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting menu Id: {Id}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Mueve un menú hacia arriba en el orden (entre sus hermanos).
    /// </summary>
    [HttpPatch("{id:int}/move-up")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> MoveUp(int id)
    {
        try
        {
            await _menuService.MoveUpAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Menu order updated", 200));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(ApiResponse<object>.NotFoundResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving menu Id: {Id} up", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Mueve un menú hacia abajo en el orden (entre sus hermanos).
    /// </summary>
    [HttpPatch("{id:int}/move-down")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> MoveDown(int id)
    {
        try
        {
            await _menuService.MoveDownAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Menu order updated", 200));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(ApiResponse<object>.NotFoundResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving menu Id: {Id} down", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Obtiene los menús del usuario actual autenticado
    /// </summary>
    [HttpGet("usuario-actual")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MenuResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MenuResponseDto>>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MenuResponseDto>>>> GetMenusUsuarioActual()
    {
        try
        {
            // Obtener el UserTypeId del token JWT
            var tipoUsuarioIdClaim = User.FindFirst("UserTypeId")?.Value;
            
            if (string.IsNullOrEmpty(tipoUsuarioIdClaim) || !int.TryParse(tipoUsuarioIdClaim, out int tipoUsuarioId))
            {
                return Unauthorized(ApiResponse<IEnumerable<MenuResponseDto>>.ErrorResponse(
                    "Invalid token or missing user type information",
                    new List<string> { "Could not retrieve user type from token" },
                    401));
            }

            var result = await _menuService.GetMenusUsuarioActualAsync(tipoUsuarioId);
            return Ok(ApiResponse<IEnumerable<MenuResponseDto>>.SuccessResponse(
                result,
                "User menus retrieved successfully",
                200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current user menus");
            return StatusCode(500, ApiResponse<IEnumerable<MenuResponseDto>>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }
}
