using Amazon.S3;
using Amazon.SQS;
using DataInsight.src.DataInsight.Api.Application.Interfaces;
using DataInsight.src.DataInsight.Api.Application.Services;
using DataInsight.src.DataInsight.Api.Infrastructure.Messaging;
using DataInsight.src.DataInsight.Api.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace DataInsight.src.DataInsight.Api.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddSingleton<IAmazonSQS, AmazonSQSClient>();
        services.AddSingleton<IAmazonS3, AmazonS3Client>();

        services.AddSingleton<ISqsPublisher, SqsPublisher>();
        services.AddSingleton<IUploadUrlGenerator, S3UploadUrlGenerator>();

        services.AddSingleton<IJobService, JobService>();
        services.AddSingleton<IUploadService, UploadService>();
        services.AddSingleton<IJobResultService, JobResultService>();
        return services;
    }
}