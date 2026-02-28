using AutoMapper;
using TherapuHubAPI.DTOs.Requests.Events;
using TherapuHubAPI.DTOs.Responses.Events;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Services;

public class TipoEventoService : ITipoEventoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TipoEventoService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TipoEventoResponseDto>> GetTiposEventoActivosAsync()
    {
        var tipos = await _unitOfWork.EventTypes.FindAsync(t => t.IsActive);
        return _mapper.Map<IEnumerable<TipoEventoResponseDto>>(tipos);
    }

    public async Task<IEnumerable<TipoEventoResponseDto>> GetAllAsync()
    {
        var tipos = await _unitOfWork.EventTypes.GetAllAsync();
        return _mapper.Map<IEnumerable<TipoEventoResponseDto>>(tipos);
    }

    public async Task<TipoEventoResponseDto?> GetByIdAsync(int id)
    {
        var tipo = await _unitOfWork.EventTypes.GetByIdAsync(id);
        return tipo == null ? null : _mapper.Map<TipoEventoResponseDto>(tipo);
    }

    public async Task<TipoEventoResponseDto> CreateAsync(CreateEventTypeRequestDto dto)
    {
        var entity = _mapper.Map<EventTypes>(dto);
        await _unitOfWork.EventTypes.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TipoEventoResponseDto>(entity);
    }

    public async Task<TipoEventoResponseDto?> UpdateAsync(int id, UpdateEventTypeRequestDto dto)
    {
        var entity = await _unitOfWork.EventTypes.GetByIdAsync(id);
        if (entity == null) return null;

        _mapper.Map(dto, entity);
        _unitOfWork.EventTypes.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TipoEventoResponseDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _unitOfWork.EventTypes.GetByIdAsync(id);
        if (entity == null) return false;

        _unitOfWork.EventTypes.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<TipoEventoResponseDto?> ToggleActiveAsync(int id)
    {
        var entity = await _unitOfWork.EventTypes.GetByIdAsync(id);
        if (entity == null) return null;

        entity.IsActive = !entity.IsActive;
        _unitOfWork.EventTypes.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TipoEventoResponseDto>(entity);
    }
}
