using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.DTOs.Requests.Clients;
using TherapuHubAPI.DTOs.Responses.Clients;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio;
using TherapuHubAPI.Repositorio.IRepositorio;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Services;

public class ClientService : IClientService
{
    private readonly IClientRepositorio _clientRepositorio;
    private readonly IClientStatusRepositorio _statusRepositorio;
    private readonly IStaffRepositorio _staffRepositorio;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ContextDB _context;
    private readonly ILogger<ClientService> _logger;

    public ClientService(
        IClientRepositorio clientRepositorio,
        IClientStatusRepositorio statusRepositorio,
        IStaffRepositorio staffRepositorio,
        IUnitOfWork unitOfWork,
        ContextDB context,
        ILogger<ClientService> logger)
    {
        _clientRepositorio = clientRepositorio;
        _statusRepositorio = statusRepositorio;
        _staffRepositorio = staffRepositorio;
        _unitOfWork = unitOfWork;
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ClientResponseDto>> GetByCompanyIdAsync(int companyId)
    {
        var clients = await _clientRepositorio.GetByCompanyIdAsync(companyId);
        var statuses = (await _statusRepositorio.GetAllAsync()).ToList();
        var staff = (await _staffRepositorio.GetByCompanyIdAsync(companyId)).ToList();
        return clients.Select(c => MapToDto(c, statuses, staff));
    }

    public async Task<ClientResponseDto?> GetByIdAsync(int id, int companyId)
    {
        var client = await _clientRepositorio.GetByIdAndCompanyAsync(id, companyId);
        if (client == null) return null;
        var statuses = (await _statusRepositorio.GetAllAsync()).ToList();
        var staff = (await _staffRepositorio.GetByCompanyIdAsync(companyId)).ToList();
        return MapToDto(client, statuses, staff);
    }

    public async Task<ClientResponseDto> CreateAsync(CreateClientRequestDto request, int companyId)
    {
        _logger.LogInformation("Creating client {FullName} for company {CompanyId}", request.FullName, companyId);

        var clientCode = await _clientRepositorio.GenerateClientCodeAsync(companyId);

        var actor = new Actors
        {
            ActorType = "CLIENT",
            FullName = request.FullName.Trim(),
            Email = request.Email?.Trim(),
            Phone = request.Phone?.Trim(),
            CompanyId = companyId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        var client = new Clients
        {
            ClientCode = clientCode,
            BirthDate = request.BirthDate,
            GuardianName = request.GuardianName?.Trim(),
            ClientStatusId = request.ClientStatusId,
            RBTId = request.RBTId,
            Emoji = request.Emoji,
            CreatedAt = DateTime.UtcNow,
            Actor = actor,
        };

        _context.Actors.Add(actor);
        await _clientRepositorio.AddAsync(client);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Client created with Id: {Id}, Code: {Code}", client.Id, client.ClientCode);

        var statuses = (await _statusRepositorio.GetAllAsync()).ToList();
        var staff = (await _staffRepositorio.GetByCompanyIdAsync(companyId)).ToList();
        return MapToDto(client, statuses, staff);
    }

    public async Task<ClientResponseDto?> UpdateAsync(int id, UpdateClientRequestDto request, int companyId)
    {
        var client = await _clientRepositorio.GetByIdAndCompanyAsync(id, companyId);
        if (client == null) return null;

        client.Actor.FullName = request.FullName.Trim();
        client.Actor.Email = request.Email?.Trim();
        client.Actor.Phone = request.Phone?.Trim();
        client.BirthDate = request.BirthDate ?? client.BirthDate;
        client.GuardianName = request.GuardianName?.Trim();
        client.ClientStatusId = request.ClientStatusId;
        client.RBTId = request.RBTId;
        client.Emoji = request.Emoji;

        _clientRepositorio.Update(client);
        await _unitOfWork.SaveChangesAsync();

        var statuses = (await _statusRepositorio.GetAllAsync()).ToList();
        var staff = (await _staffRepositorio.GetByCompanyIdAsync(companyId)).ToList();
        return MapToDto(client, statuses, staff);
    }

    public async Task<bool> DeleteAsync(int id, int companyId)
    {
        var client = await _clientRepositorio.GetByIdAndCompanyAsync(id, companyId);
        if (client == null) return false;
        _clientRepositorio.Remove(client);
        _context.Actors.Remove(client.Actor);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ClientStatusResponseDto>> GetAllStatusesAsync()
    {
        var list = await _statusRepositorio.GetAllAsync();
        return list.Select(s => new ClientStatusResponseDto
        {
            Id = s.Id,
            Name = s.Name,
            Code = s.Code,
            IsActive = s.IsActive
        });
    }

    private static ClientResponseDto MapToDto(Clients c, List<ClientStatuses> statuses, List<Staff> staff)
    {
        var status = statuses.FirstOrDefault(x => x.Id == c.ClientStatusId);
        var rbt = c.RBTId.HasValue ? staff.FirstOrDefault(x => x.Id == c.RBTId.Value) : null;
        return new ClientResponseDto
        {
            Id = c.Id,
            ActorId = c.ActorId,
            ClientCode = c.ClientCode,
            FullName = c.Actor.FullName,
            BirthDate = c.BirthDate,
            Email = c.Actor.Email,
            Phone = c.Actor.Phone,
            GuardianName = c.GuardianName,
            ClientStatusId = c.ClientStatusId,
            ClientStatusName = status?.Name,
            CompanyId = c.Actor.CompanyId,
            RBTId = c.RBTId,
            RBTName = rbt?.Actor.FullName,
            Emoji = c.Emoji,
            CreatedAt = c.CreatedAt,
        };
    }
}
