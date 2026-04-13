using TherapuHubAPI.DTOs.Requests.Clients;
using TherapuHubAPI.DTOs.Responses.Clients;

namespace TherapuHubAPI.Services.IServices;

public interface IClientService
{
    Task<IEnumerable<ClientResponseDto>> GetByCompanyIdAsync(int companyId, int userId);
    Task<ClientResponseDto?> GetByIdAsync(int id, int companyId);
    Task<ClientResponseDto> CreateAsync(CreateClientRequestDto request, int companyId);
    Task<ClientResponseDto?> UpdateAsync(int id, UpdateClientRequestDto request, int companyId);
    Task<bool> DeleteAsync(int id, int companyId);
    Task<IEnumerable<ClientStatusResponseDto>> GetAllStatusesAsync();
}
