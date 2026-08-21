using Amazon.S3;
using DataInsight.Worker.Application.Interfaces;
using DataInsight.Worker.Application.Services;
using DataInsight.Worker.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace DataInsight.Worker.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkerServices(
        this IServiceCollection services)
    {
        services.AddSingleton<IAmazonS3, AmazonS3Client>();

        services.AddSingleton<IS3StorageService, S3StorageService>();

        services.AddSingleton<
            IFileProcessingService,
            FileProcessingService>();

        return services;
    }
}