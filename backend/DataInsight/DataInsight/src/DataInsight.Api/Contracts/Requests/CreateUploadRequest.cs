namespace DataInsight.src.DataInsight.Api.Contracts.Requests;

public class CreateUploadRequest
{
    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;
}