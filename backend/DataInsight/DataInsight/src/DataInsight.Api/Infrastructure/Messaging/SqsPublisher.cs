using Amazon.SQS;
using Amazon.SQS.Model;
using DataInsight.src.DataInsight.Api.Messages;
using System.Text.Json;

namespace DataInsight.src.DataInsight.Api.Infrastructure.Messaging;

public class SqsPublisher : ISqsPublisher
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueUrl;

    public SqsPublisher(IAmazonSQS sqsClient)
    {
        _sqsClient = sqsClient;

        _queueUrl =
            Environment.GetEnvironmentVariable("PROCESSING_QUEUE_URL")
            ?? throw new InvalidOperationException(
                "PROCESSING_QUEUE_URL environment variable is not configured.");
    }

    public async Task PublishAsync(ProcessFileMessage message)
    {
        var messageBody = JsonSerializer.Serialize(message);

        var request = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = messageBody
        };

        await _sqsClient.SendMessageAsync(request);
    }
}