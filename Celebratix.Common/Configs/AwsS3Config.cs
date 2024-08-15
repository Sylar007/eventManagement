namespace Celebratix.Common.Configs;

public class AwsS3Config
{
    public string AccessKey { get; set; } = null!;

    public string AccessSecret { get; set; } = null!;

    public string Endpoint { get; set; } = null!;

    public string FileBucketName { get; set; } = null!;

    public string ImageCacheBucketName { get; set; } = null!;

    public string ImageDirectory { get; set; } = null!;

    public string Region { get; set; } = null!;
}
