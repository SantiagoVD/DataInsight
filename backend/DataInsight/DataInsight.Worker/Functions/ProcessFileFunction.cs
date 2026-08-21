using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using DataInsight.Worker.Application.Interfaces;
using DataInsight.Worker.Messages;
using System.Text.Json;

namespace DataInsight.Worker.Functions;

public class ProcessFileFunction
{
    private readonly IFileProcessingService _fileProcessingService;

    public ProcessFileFunction(
        IFileProcessingService fileProcessingService)
    {
        _fileProcessingService = fileProcessingService;
    }

    public async Task HandleAsync(
        SQSEvent sqsEvent,
        ILambdaContext context)
    {
        foreach (var record in sqsEvent.Records)
        {
            var message =
                JsonSerializer.Deserialize<ProcessFileMessage>(
                    record.Body,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (message is null)
            {
                throw new InvalidOperationException(
                    "Unable to deserialize SQS message.");
            }

            await _fileProcessingService.ProcessAsync(message);
        }
    }
}