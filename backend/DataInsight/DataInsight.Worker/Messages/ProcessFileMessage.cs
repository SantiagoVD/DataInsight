namespace DataInsight.Worker.Messages;

public class ProcessFileMessage
{
    public string JobId { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public string BucketName { get; set; } = string.Empty;

    public string ObjectKey { get; set; } = string.Empty;
}