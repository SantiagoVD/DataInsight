namespace DataInsight.src.DataInsight.Api.Contracts.Responses;

public class CreateUploadResponse
{
    public string UploadUrl { get; set; } = string.Empty;

    public string ObjectKey { get; set; } = string.Empty;
}