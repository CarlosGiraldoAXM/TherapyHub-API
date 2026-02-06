using AutoMapper;
using TherapuHubAPI.DTOs.Responses.Events;
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
}
