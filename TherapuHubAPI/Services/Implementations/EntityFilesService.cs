using TherapuHubAPI.DTOs.Responses.Files;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio;
using TherapuHubAPI.Repositorio.IRepositorio;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Services.Implementations;

public class EntityFilesService : IEntityFilesService
{
    private readonly IFileRepositorio _fileRepo;
    private readonly IFileStorageService _storage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EntityFilesService> _logger;

    public EntityFilesService(
        IFileRepositorio fileRepo,
        IFileStorageService storage,
        IUnitOfWork unitOfWork,
        ILogger<EntityFilesService> logger)
    {
        _fileRepo = fileRepo;
        _storage = storage;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<EntityFileResponseDto>> GetByOwnerAndTypeAsync(int ownerActorId, int filesTypeId)
    {
        var files = await _fileRepo.GetByOwnerAndTypeAsync(ownerActorId, filesTypeId);
        var result = new List<EntityFileResponseDto>();

        foreach (var f in files)
        {
            long size = 0;
            try { size = await _storage.GetFileSizeAsync(f.BlobPath); }
            catch { /* file may not exist on disk in edge cases */ }

            result.Add(MapToDto(f, size));
        }

        return result;
    }

    public async Task<EntityFileResponseDto> UploadAsync(IFormFile file, int ownerActorId, int filesTypeId, int uploaderActorId, int companyId)
    {
        if (file.Length == 0)
            throw new InvalidOperationException("The uploaded file is empty");

        // Container path: company/actors/{ownerActorId}/{filesTypeId}
        var containerPath = $"{companyId}/actors/{ownerActorId}/{filesTypeId}";
        var blobPath = await _storage.SaveFileAsync(file, containerPath);

        var entity = new Files
        {
            FolderId = null,
            FileName = file.FileName,
            BlobPath = blobPath,
            UploadedByActorId = uploaderActorId,
            OwnerActorId = ownerActorId,
            FilesTypeId = filesTypeId,
            UploadedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _fileRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Entity file '{Name}' uploaded for actor {OwnerActorId}, type {TypeId}, Id: {FileId}",
            file.FileName, ownerActorId, filesTypeId, entity.Id);

        return MapToDto(entity, file.Length);
    }

    public async Task<(Stream Stream, string ContentType, string FileName)?> DownloadAsync(long fileId)
    {
        var file = await _fileRepo.GetByIdAsync(fileId);
        if (file == null) return null;

        try
        {
            return await _storage.GetFileAsync(file.BlobPath);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    public async Task<bool> DeleteAsync(long fileId, int deleterActorId)
    {
        var file = await _fileRepo.GetByIdAsync(fileId);
        if (file == null) return false;

        try { await _storage.DeleteFileAsync(file.BlobPath); }
        catch (Exception ex) { _logger.LogWarning(ex, "Could not delete blob for file Id {FileId}", fileId); }

        file.IsDeleted = true;
        file.DeletedAt = DateTime.UtcNow;
        file.DeletedActorId = deleterActorId;
        _fileRepo.Update(file);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Entity file {Id} soft-deleted by actor {ActorId}", fileId, deleterActorId);
        return true;
    }

    private EntityFileResponseDto MapToDto(Files f, long size) => new()
    {
        Id = f.Id,
        FileName = f.FileName,
        FileSize = size,
        ContentType = _storage.ResolveContentType(f.FileName),
        UploadedByActorId = f.UploadedByActorId,
        UploadedAt = f.UploadedAt,
        OwnerActorId = f.OwnerActorId ?? 0,
        FilesTypeId = f.FilesTypeId ?? 0
    };
}
