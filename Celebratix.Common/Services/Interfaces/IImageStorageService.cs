using Celebratix.Common.Models.DbModels;
using Microsoft.AspNetCore.Http;

namespace Celebratix.Common.Services.Interfaces;

public interface IImageStorageService
{
    /// <summary>
    /// Creates the file model without saving it or attaching it to any DbContext
    /// </summary>
    public Task<ImageDbModel?> UploadFile(IFormFile? file);

    /// <summary>
    /// Should return true if deletion was successful
    /// </summary>
    public Task DeleteFile(Guid fileId);
}