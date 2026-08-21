using DataInsight.src.DataInsight.Api.Application.Interfaces;
using DataInsight.src.DataInsight.Api.Contracts.Requests;
using DataInsight.src.DataInsight.Api.Contracts.Responses;
using DataInsight.src.DataInsight.Api.Infrastructure.Storage;

namespace DataInsight.src.DataInsight.Api.Application.Services;

public class UploadService : IUploadService
{
    private readonly IUploadUrlGenerator _uploadUrlGenerator;

    public UploadService(
        IUploadUrlGenerator uploadUrlGenerator)
    {
        _uploadUrlGenerator = uploadUrlGenerator;
    }

    public async Task<CreateUploadResponse> CreateUploadUrlAsync(
        CreateUploadRequest request)
    {
        var bucketName =
            Environment.GetEnvironmentVariable("DATA_BUCKET_NAME")
            ?? throw new InvalidOperationException(
                "DATA_BUCKET_NAME environment variable is not configured.");

        var uploadId = Guid.NewGuid().ToString();

        var objectKey =
            $"uploads/{uploadId}/{request.FileName}";

        var uploadUrl =
            await _uploadUrlGenerator.GenerateUploadUrlAsync(
                bucketName,
                objectKey,
                request.ContentType);

        return new CreateUploadResponse
        {
            UploadUrl = uploadUrl,
            ObjectKey = objectKey
        };
    }
}