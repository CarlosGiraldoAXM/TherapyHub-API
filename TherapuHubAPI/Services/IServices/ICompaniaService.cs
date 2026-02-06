using TherapuHubAPI.DTOs.Requests.Companies;
using TherapuHubAPI.DTOs.Responses.Companies;

namespace TherapuHubAPI.Services.IServices;

public interface ICompaniaService
{
    Task<CompaniaResponseDto> CreateAsync(CreateCompaniaRequestDto request);
    Task<CompaniaResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<CompaniaResponseDto>> GetAllAsync();
    Task<CompaniaResponseDto?> UpdateAsync(int id, UpdateCompaniaRequestDto request);
    Task<bool> DeleteAsync(int id);
    Task<bool> ToggleActivoAsync(int id);
}
