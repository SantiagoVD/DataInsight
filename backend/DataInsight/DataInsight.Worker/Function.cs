using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using DataInsight.Worker.Application.Interfaces;
using DataInsight.Worker.DependencyInjection;
using DataInsight.Worker.Functions;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(
    typeof(
        Amazon.Lambda.Serialization.SystemTextJson
            .DefaultLambdaJsonSerializer
    )
)]

namespace DataInsight.Worker;

public class Function
{
    private readonly ProcessFileFunction _processFileFunction;

    public Function()
    {
        var services = new ServiceCollection();

        services.AddWorkerServices();

        var serviceProvider =
            services.BuildServiceProvider();

        var fileProcessingService =
            serviceProvider
                .GetRequiredService<IFileProcessingService>();

        _processFileFunction =
            new ProcessFileFunction(
                fileProcessingService);
    }

    public async Task FunctionHandler(
        SQSEvent sqsEvent,
        ILambdaContext context)
    {
        await _processFileFunction.HandleAsync(
            sqsEvent,
            context);
    }
}