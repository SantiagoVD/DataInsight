using Amazon.S3;
using Amazon.S3.Model;

namespace DataInsight.src.DataInsight.Api.Infrastructure.Storage;

public class S3UploadUrlGenerator : IUploadUrlGenerator
{
    private readonly IAmazonS3 _s3Client;

    public S3UploadUrlGenerator(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public Task<string> GenerateUploadUrlAsync(
        string bucketName,
        string objectKey,
        string contentType)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = DateTime.UtcNow.AddMinutes(5)
        };

        var uploadUrl =
            _s3Client.GetPreSignedURL(request);

        return Task.FromResult(uploadUrl);
    }
}