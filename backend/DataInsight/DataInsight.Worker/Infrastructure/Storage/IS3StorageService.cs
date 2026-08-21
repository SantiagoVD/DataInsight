namespace DataInsight.Worker.Infrastructure.Storage;

public interface IS3StorageService
{
    Task<string> GetFileContentAsync(
        string bucketName,
        string objectKey);

    Task SaveResultAsync(
        string bucketName,
        string objectKey,
        string content);
}