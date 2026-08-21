using DataInsight.src.DataInsight.Api.Application.Interfaces;
using DataInsight.src.DataInsight.Api.Contracts.Requests;
using DataInsight.src.DataInsight.Api.Contracts.Responses;
using DataInsight.src.DataInsight.Api.Domain.Entities;
using DataInsight.src.DataInsight.Api.Domain.Enums;
using DataInsight.src.DataInsight.Api.Infrastructure.Messaging;
using DataInsight.src.DataInsight.Api.Messages;

namespace DataInsight.src.DataInsight.Api.Application.Services;

public class JobService : IJobService
{
    private readonly ISqsPublisher _sqsPublisher;

    public JobService(ISqsPublisher sqsPublisher)
    {
        _sqsPublisher = sqsPublisher;
    }

    public async Task<CreateJobResponse> CreateJobAsync(CreateJobRequest request)
    {
        var bucketName =
            Environment.GetEnvironmentVariable("DATA_BUCKET_NAME")
            ?? throw new InvalidOperationException(
                "DATA_BUCKET_NAME environment variable is not configured.");

        var job = new ProcessingJob
        {
            JobId = Guid.NewGuid().ToString(),
            FileName = request.FileName,
            ContentType = request.ContentType,
            Status = JobStatus.Queued,
            CreatedAt = DateTime.UtcNow
        };

        var message = new ProcessFileMessage
        {
            JobId = job.JobId,
            FileName = job.FileName,
            ContentType = job.ContentType,
            BucketName = bucketName,
            ObjectKey = request.ObjectKey
        };

        await _sqsPublisher.PublishAsync(message);

        return new CreateJobResponse
        {
            JobId = job.JobId,
            Status = job.Status.ToString().ToUpperInvariant()
        };
    }
}