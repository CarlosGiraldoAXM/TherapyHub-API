using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.Users;
using TherapuHubAPI.DTOs.Responses.Users;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(
        IUsuarioService usuarioService,
        ILogger<UsuariosController> logger)
    {
        _usuarioService = usuarioService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new user. System-type users can pass CompanyId to create users for any company; others create users in their own company.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UsuarioResponseDto>>> Create([FromBody] CreateUsuarioRequestDto request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized(ApiResponse<UsuarioResponseDto>.ErrorResponse(
                    "User could not be determined. Please log in again.",
                    new List<string> { "Invalid or missing user context" },
                    401));
            }

            var result = await _usuarioService.CreateAsync(request, currentUserId);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                ApiResponse<UsuarioResponseDto>.SuccessResponse(
                    result,
                    "User created successfully",
                    201));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al crear usuario");
            return BadRequest(ApiResponse<UsuarioResponseDto>.ErrorResponse(
                ex.Message,
                new List<string> { ex.Message },
                400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            return StatusCode(500, ApiResponse<UsuarioResponseDto>.ErrorResponse(
                "Error interno del servidor",
                new List<string> { "Ocurrió un error al procesar la solicitud" },
                500));
        }
    }

    /// <summary>
    /// Gets all users. System-type users see all users; other users see only users from the same company.
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UsuarioResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UsuarioResponseDto>>>> GetAll()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized(ApiResponse<IEnumerable<UsuarioResponseDto>>.ErrorResponse(
                    "User could not be determined. Please log in again.",
                    new List<string> { "Invalid or missing user context" },
                    401));
            }

            var result = await _usuarioService.GetAllAsync(currentUserId);
            return Ok(ApiResponse<IEnumerable<UsuarioResponseDto>>.SuccessResponse(
                result,
                "Users retrieved successfully",
                200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, ApiResponse<IEnumerable<UsuarioResponseDto>>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Cambia el estado activo/inactivo de un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPatch("{id}/toggle-activo")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UsuarioResponseDto>>> ToggleActivo(int id)
    {
        try
        {
            var result = await _usuarioService.ToggleActivoAsync(id);

            if (!result)
            {
                return NotFound(ApiResponse<UsuarioResponseDto>.NotFoundResponse(
                    $"Usuario con Id {id} no encontrado"));
            }

            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null)
            {
                return NotFound(ApiResponse<UsuarioResponseDto>.NotFoundResponse(
                    $"User with Id {id} not found"));
            }
            return Ok(ApiResponse<UsuarioResponseDto>.SuccessResponse(
                usuario,
                "User status updated successfully",
                200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user status with Id: {Id}", id);
            return StatusCode(500, ApiResponse<UsuarioResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Obtiene un usuario por ID
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <returns>Usuario encontrado</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UsuarioResponseDto>>> GetById(int id)
    {
        try
        {
            var result = await _usuarioService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<UsuarioResponseDto>.NotFoundResponse(
                    $"User with Id {id} not found"));
            }

            return Ok(ApiResponse<UsuarioResponseDto>.SuccessResponse(
                result,
                "User retrieved successfully",
                200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with Id: {Id}", id);
            return StatusCode(500, ApiResponse<UsuarioResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Actualiza un usuario existente
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="request">Datos del usuario a actualizar</param>
    /// <returns>Usuario actualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<UsuarioResponseDto>>> Update(int id, [FromBody] UpdateUsuarioRequestDto request)
    {
        try
        {
            var result = await _usuarioService.UpdateAsync(id, request);

            if (result == null)
            {
                return NotFound(ApiResponse<UsuarioResponseDto>.NotFoundResponse(
                    $"User with Id {id} not found"));
            }

            return Ok(ApiResponse<UsuarioResponseDto>.SuccessResponse(
                result,
                "User updated successfully",
                200));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error updating user with Id: {Id}", id);
            return BadRequest(ApiResponse<UsuarioResponseDto>.ErrorResponse(
                ex.Message,
                new List<string> { ex.Message },
                400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with Id: {Id}", id);
            return StatusCode(500, ApiResponse<UsuarioResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Elimina un usuario (soft delete)
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse(
                    "User could not be determined. Please log in again.",
                    new List<string> { "Invalid or missing user context" },
                    401));
            }

            var result = await _usuarioService.DeleteAsync(id, currentUserId);

            if (!result)
            {
                return NotFound(ApiResponse<object>.NotFoundResponse(
                    $"User with Id {id} not found"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(
                null,
                "User deleted successfully",
                200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with Id: {Id}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Resets a user's password to the default (123456) and sets RequiresPasswordReset = true. The user must set a new password on next login.
    /// </summary>
    [HttpPost("{id}/reset-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> ResetPassword(int id)
    {
        try
        {
            await _usuarioService.ResetPasswordAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(
                null,
                "Password has been reset. User must set a new password on next login.",
                200));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error resetting password for user Id: {Id}", id);
            return BadRequest(ApiResponse<object>.ErrorResponse(
                ex.Message,
                new List<string> { ex.Message },
                400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user with Id: {Id}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }
}
