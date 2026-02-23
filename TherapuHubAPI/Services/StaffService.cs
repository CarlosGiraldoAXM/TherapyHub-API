using TherapuHubAPI.DTOs.Requests.Staff;
using TherapuHubAPI.DTOs.Responses.Staff;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio;
using TherapuHubAPI.Repositorio.IRepositorio;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Services;

public class StaffService : IStaffService
{
    private readonly IStaffRepositorio _staffRepositorio;
    private readonly IStaffStatusRepositorio _statusRepositorio;
    private readonly IStaffRolesRepositorio _rolesRepositorio;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StaffService> _logger;

    public StaffService(
        IStaffRepositorio staffRepositorio,
        IStaffStatusRepositorio statusRepositorio,
        IStaffRolesRepositorio rolesRepositorio,
        IUnitOfWork unitOfWork,
        ILogger<StaffService> logger)
    {
        _staffRepositorio = staffRepositorio;
        _statusRepositorio = statusRepositorio;
        _rolesRepositorio = rolesRepositorio;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<StaffResponseDto>> GetByCompanyIdAsync(int companyId)
    {
        var staff = await _staffRepositorio.GetByCompanyIdAsync(companyId);
        var statuses = (await _statusRepositorio.GetAllAsync()).ToList();
        var roles = (await _rolesRepositorio.GetAllAsync()).ToList();
        return staff.Select(s => MapToDto(s, statuses, roles));
    }

    public async Task<StaffResponseDto?> GetByIdAsync(int id, int companyId)
    {
        var staff = await _staffRepositorio.GetByIdAndCompanyAsync(id, companyId);
        if (staff == null) return null;
        var statuses = (await _statusRepositorio.GetAllAsync()).ToList();
        var roles = (await _rolesRepositorio.GetAllAsync()).ToList();
        return MapToDto(staff, statuses, roles);
    }

    public async Task<StaffResponseDto> CreateAsync(CreateStaffRequestDto request, int companyId)
    {
        _logger.LogInformation("Creating staff {FirstName} {LastName} for company {CompanyId}", request.FirstName, request.LastName, companyId);

        var contractDate = request.ContractDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var dateOfBirth = request.DateOfBirth ?? DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30));

        var staff = new Staff
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            RoleId = request.RoleId,
            CompanyId = companyId,
            DateOfBirth = dateOfBirth,
            Phone = request.Phone.Trim(),
            Email = request.Email.Trim(),
            StatusId = request.StatusId,
            ContractDate = contractDate,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _staffRepositorio.AddAsync(staff);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Staff created with Id: {Id}", staff.Id);
        var statuses = (await _statusRepositorio.GetAllAsync()).ToList();
        var roles = (await _rolesRepositorio.GetAllAsync()).ToList();
        return MapToDto(staff, statuses, roles);
    }

    public async Task<StaffResponseDto?> UpdateAsync(int id, UpdateStaffRequestDto request, int companyId)
    {
        _logger.LogInformation("Updating staff Id: {Id}", id);

        var staff = await _staffRepositorio.GetByIdAndCompanyAsync(id, companyId);
        if (staff == null)
        {
            _logger.LogWarning("Staff not found Id: {Id}", id);
            return null;
        }

        staff.FirstName = request.FirstName.Trim();
        staff.LastName = request.LastName.Trim();
        staff.RoleId = request.RoleId;
        staff.DateOfBirth = request.DateOfBirth ?? staff.DateOfBirth;
        staff.Phone = request.Phone.Trim();
        staff.Email = request.Email.Trim();
        staff.StatusId = request.StatusId;
        staff.ContractDate = request.ContractDate ?? staff.ContractDate;

        _staffRepositorio.Update(staff);
        await _unitOfWork.SaveChangesAsync();

        var statuses = (await _statusRepositorio.GetAllAsync()).ToList();
        var roles = (await _rolesRepositorio.GetAllAsync()).ToList();
        return MapToDto(staff, statuses, roles);
    }

    public async Task<bool> DeleteAsync(int id, int companyId)
    {
        var staff = await _staffRepositorio.GetByIdAndCompanyAsync(id, companyId);
        if (staff == null) return false;
        _staffRepositorio.Remove(staff);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Staff deleted Id: {Id}", id);
        return true;
    }

    public async Task<bool> ToggleActiveAsync(int id, int companyId)
    {
        var staff = await _staffRepositorio.GetByIdAndCompanyAsync(id, companyId);
        if (staff == null) return false;
        staff.IsActive = !staff.IsActive;
        _staffRepositorio.Update(staff);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Staff IsActive toggled Id: {Id}, IsActive: {IsActive}", id, staff.IsActive);
        return true;
    }

    public async Task<IEnumerable<StaffStatusResponseDto>> GetAllStatusesAsync()
    {
        var list = await _statusRepositorio.GetAllAsync();
        return list.Select(s => new StaffStatusResponseDto { Id = s.Id, Name = s.Name, IsActive = s.IsActive });
    }

    public async Task<IEnumerable<StaffRoleResponseDto>> GetAllRolesAsync()
    {
        var list = await _rolesRepositorio.GetAllAsync();
        return list.Select(r => new StaffRoleResponseDto { Id = r.Id, Name = r.Name, IsActive = r.IsActive });
    }

    private static StaffResponseDto MapToDto(Staff s, List<StaffStatus> statuses, List<StaffRoles> roles)
    {
        var status = statuses.FirstOrDefault(x => x.Id == s.StatusId);
        var role = roles.FirstOrDefault(x => x.Id == s.RoleId);
        return new StaffResponseDto
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            RoleId = s.RoleId,
            RoleName = role?.Name,
            CompanyId = s.CompanyId,
            DateOfBirth = s.DateOfBirth,
            Phone = s.Phone,
            Email = s.Email,
            StatusId = s.StatusId,
            StatusName = status?.Name,
            ContractDate = s.ContractDate,
            CreatedAt = s.CreatedAt,
            IsActive = s.IsActive
        };
    }
}
