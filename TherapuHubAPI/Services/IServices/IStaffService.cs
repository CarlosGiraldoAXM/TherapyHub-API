using TherapuHubAPI.DTOs.Requests.Staff;
using TherapuHubAPI.DTOs.Responses.Staff;

namespace TherapuHubAPI.Services.IServices;

public interface IStaffService
{
    Task<IEnumerable<StaffResponseDto>> GetByCompanyIdAsync(int companyId);
    Task<StaffResponseDto?> GetByIdAsync(int id, int companyId);
    Task<StaffResponseDto> CreateAsync(CreateStaffRequestDto request, int companyId);
    Task<StaffResponseDto?> UpdateAsync(int id, UpdateStaffRequestDto request, int companyId);
    Task<bool> DeleteAsync(int id, int companyId);
    Task<bool> ToggleActiveAsync(int id, int companyId);
    Task<IEnumerable<StaffStatusResponseDto>> GetAllStatusesAsync();
    Task<IEnumerable<StaffRoleResponseDto>> GetAllRolesAsync();
}
