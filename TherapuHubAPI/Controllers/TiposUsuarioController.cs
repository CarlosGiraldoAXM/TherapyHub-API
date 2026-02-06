using Microsoft.AspNetCore.Mvc;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.UserTypes;
using TherapuHubAPI.DTOs.Responses.UserTypes;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TiposUsuarioController : ControllerBase
{
    private readonly ITipoUsuarioService _tipoUsuarioService;
    private readonly ILogger<TiposUsuarioController> _logger;

    public TiposUsuarioController(
        ITipoUsuarioService tipoUsuarioService,
        ILogger<TiposUsuarioController> logger)
    {
        _tipoUsuarioService = tipoUsuarioService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los tipos de usuario
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TipoUsuarioResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<TipoUsuarioResponseDto>>>> GetAll()
    {
        try
        {
            var result = await _tipoUsuarioService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<TipoUsuarioResponseDto>>.SuccessResponse(
                result,
                "User types retrieved successfully",
                200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user types");
            return StatusCode(500, ApiResponse<IEnumerable<TipoUsuarioResponseDto>>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Obtiene un tipo de usuario por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TipoUsuarioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TipoUsuarioResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TipoUsuarioResponseDto>>> GetById(int id)
    {
        try
        {
            var result = await _tipoUsuarioService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<TipoUsuarioResponseDto>.NotFoundResponse(
                    $"User type with Id {id} not found"));
            }

            return Ok(ApiResponse<TipoUsuarioResponseDto>.SuccessResponse(
                result,
                "User type retrieved successfully",
                200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user type with Id: {Id}", id);
            return StatusCode(500, ApiResponse<TipoUsuarioResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Crea un nuevo tipo de usuario
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TipoUsuarioResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<TipoUsuarioResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<TipoUsuarioResponseDto>>> Create([FromBody] CreateTipoUsuarioRequestDto request)
    {
        try
        {
            var result = await _tipoUsuarioService.CreateAsync(request);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                ApiResponse<TipoUsuarioResponseDto>.SuccessResponse(
                    result,
                    "User type created successfully",
                    201));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error creating user type");
            return BadRequest(ApiResponse<TipoUsuarioResponseDto>.ErrorResponse(
                ex.Message,
                new List<string> { ex.Message },
                400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user type");
            return StatusCode(500, ApiResponse<TipoUsuarioResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Actualiza un tipo de usuario existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TipoUsuarioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TipoUsuarioResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<TipoUsuarioResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<TipoUsuarioResponseDto>>> Update(int id, [FromBody] UpdateTipoUsuarioRequestDto request)
    {
        try
        {
            var result = await _tipoUsuarioService.UpdateAsync(id, request);

            if (result == null)
            {
                return NotFound(ApiResponse<TipoUsuarioResponseDto>.NotFoundResponse(
                    $"User type with Id {id} not found"));
            }

            return Ok(ApiResponse<TipoUsuarioResponseDto>.SuccessResponse(
                result,
                "User type updated successfully",
                200));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error updating user type with Id: {Id}", id);
            return BadRequest(ApiResponse<TipoUsuarioResponseDto>.ErrorResponse(
                ex.Message,
                new List<string> { ex.Message },
                400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user type with Id: {Id}", id);
            return StatusCode(500, ApiResponse<TipoUsuarioResponseDto>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }

    /// <summary>
    /// Elimina (desactiva) un tipo de usuario
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var result = await _tipoUsuarioService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(ApiResponse<object>.NotFoundResponse(
                    $"User type with Id {id} not found"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(
                null,
                "User type deactivated successfully",
                200));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error deactivating user type with Id: {Id}", id);
            return BadRequest(ApiResponse<object>.ErrorResponse(
                ex.Message,
                new List<string> { ex.Message },
                400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user type with Id: {Id}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                "Internal server error",
                new List<string> { "An error occurred while processing the request" },
                500));
        }
    }
}
