using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Celebratix.Common.Services;

public class S3ImageStorageService : IImageStorageService
{
    private readonly S3StorageService _blobStorageService;
    private readonly ILogger<S3ImageStorageService> _logger;

    public S3ImageStorageService(S3StorageService blobStorageService, ILogger<S3ImageStorageService> logger)
    {
        _blobStorageService = blobStorageService;
        _logger = logger;
    }

    public async Task<ImageDbModel?> UploadFile(IFormFile? file)
    {
        if (file == null)
            return null;

        var blobSaveResult = await _blobStorageService.SaveFileToStorage(file, _blobStorageService.Config.ImageDirectory);

        return new ImageDbModel
        {
            StorageIdentifier = blobSaveResult.Identifier,
            OriginalFileName = file.FileName,
            PermaLink = blobSaveResult.Uri.AbsoluteUri,
            Path = $"files/{blobSaveResult.Identifier}"
        };
    }

    /// <summary>
    /// Returns true if deletion succeeded
    /// Does NOT delete the db model
    /// </summary>
    public async Task DeleteFile(Guid fileId)
    {
        await _blobStorageService.DeleteFile(fileId);
    }
}
