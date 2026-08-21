using Amazon.S3;
using Amazon.S3.Model;

namespace DataInsight.Worker.Infrastructure.Storage;

public class S3StorageService : IS3StorageService
{
    private readonly IAmazonS3 _s3Client;

    public S3StorageService(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task<string> GetFileContentAsync(
        string bucketName,
        string objectKey)
    {
        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = objectKey
        };

        using var response =
            await _s3Client.GetObjectAsync(request);

        using var reader =
            new StreamReader(response.ResponseStream);

        return await reader.ReadToEndAsync();
    }

    public async Task SaveResultAsync(
        string bucketName,
        string objectKey,
        string content)
    {
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            ContentBody = content,
            ContentType = "application/json"
        };

        await _s3Client.PutObjectAsync(request);
    }
}