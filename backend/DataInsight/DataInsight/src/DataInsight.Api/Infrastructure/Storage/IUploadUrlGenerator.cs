namespace DataInsight.src.DataInsight.Api.Infrastructure.Storage;

public interface IUploadUrlGenerator
{
    Task<string> GenerateUploadUrlAsync(
        string bucketName,
        string objectKey,
        string contentType);
}