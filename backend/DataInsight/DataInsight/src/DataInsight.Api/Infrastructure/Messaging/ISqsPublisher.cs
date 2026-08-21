using DataInsight.src.DataInsight.Api.Messages;

namespace DataInsight.src.DataInsight.Api.Infrastructure.Messaging;

public interface ISqsPublisher
{
    Task PublishAsync(ProcessFileMessage message);
}