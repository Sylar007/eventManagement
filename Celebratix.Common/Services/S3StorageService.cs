using Amazon.Runtime;
using Celebratix.Common.Configs;
using FluentStorage;
using FluentStorage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Celebratix.Common.Services;

public class S3StorageService
{
    public readonly AwsS3Config Config;
    private readonly ILogger<S3StorageService> _logger;
    private IBlobStorage? _blobContainerClient;

    public S3StorageService(IOptions<AwsS3Config> config, ILogger<S3StorageService> logger)
    {
        _logger = logger;
        Config = config.Value;
    }

    /// <summary>
    /// We do not want to do this in the constructor as this service will be injected without being guaranteed
    /// (or even likely) to be used. Connecting to the service container is a bit wasteful then.
    /// </summary>
    private void InitializeBlobStorage()
    {
        if (_blobContainerClient != null)
            return;

        try
        {
            var credential = new BasicAWSCredentials(Config.AccessKey, Config.AccessSecret);

            // Get a reference to a container with the name in _config.FileContainerName and then create it
            var serviceUrl = $"https://{Config.Endpoint}";
            _blobContainerClient = StorageFactory.Blobs.AwsS3(Config.AccessKey, Config.AccessSecret, null, Config.FileBucketName, Config.Region, serviceUrl);

            // File containers should normally be private, and could configure that here with SetAccessPolicy
            // but might as well let that be handled on Azure for greater control and ease of use on dev/staging
        }
        catch (Exception e)
        {
            _logger.LogError("Connecting to blob container with name {FileContainerName} failed!\n" +
                             "Message: {Message}", Config.FileBucketName, e.Message);
            throw new ArgumentException(
                "Problem accessing file storage service! See if errors persists and then contact developers!");
        }
    }

    public async Task<BlobSaveResult> SaveFileToStorage(IFormFile file, string dir = "")
    {
        await using var stream = file.OpenReadStream();
        return await SaveFileToStorage(file.FileName, stream, dir);
    }

    public async Task<BlobSaveResult> SaveFileToStorage(string filename, Stream stream, string dir = "")
    {
        InitializeBlobStorage();
        var name = Guid.NewGuid();
        var path = Path.Combine(dir, name.ToString()).Trim('/');
        await _blobContainerClient!.WriteAsync(path, stream);

        var fileUrl = $"https://{Config.FileBucketName}.{Config.Endpoint}/{path}";
        _logger.LogInformation(
            "Successfully uploaded file fileId {Name} and original file name: \"{FileName}\" to S3 storage at {FileUrl}",
            name, filename, fileUrl);

        return new BlobSaveResult
        {
            Identifier = name,
            Uri = new Uri(fileUrl)
        };
    }

    /*
    public async Task<string> GetTemporaryDownloadLink(FileDbModel file, double expiresInMinutes = 10)
    {
        await InitializeBlobStorage();
        var blob = _blobContainerClient!.GetBlobClient(file.StorageIdentifier.ToString());

        var offset = TimeSpan.FromMinutes(expiresInMinutes);
        var expiry = DateTimeOffset.UtcNow.Add(offset);

        var sasBuilder = new BlobSasBuilder(BlobSasPermissions.Read, expiry)
        {
            ContentDisposition = "attachment; filename=" + file.OriginalFileName,
            ExpiresOn = expiry,
            CacheControl = "max-age=" + expiry // Needed for the expire to not be circumvented by caching
        };

        // Could try-catch here but might as well just let the error be handled further up by the error handler
        var uri = blob.GenerateSasUri(sasBuilder);

        return uri.AbsoluteUri;
    }
    */

    /// <summary>
    /// Returns true if deletion succeeded
    /// Does NOT delete the db model
    /// </summary>
    public async Task DeleteFile(Guid fileId)
    {
        InitializeBlobStorage();
        await _blobContainerClient!.DeleteAsync(fileId.ToString());
        _logger.LogInformation("Deleted file with fileId {FileId} from S3", fileId);
    }

    public class BlobSaveResult
    {
        public Uri Uri { get; set; } = null!;
        public Guid Identifier { get; set; }
    }
}
