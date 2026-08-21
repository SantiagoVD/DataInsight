using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
using Amazon.S3.Model;
using DataInsight.src.DataInsight.Api.Application.Interfaces;
using System.Net;
using System.Text.Json;

namespace DataInsight.src.DataInsight.Api.Application.Services;

public class JobResultService : IJobResultService
{
    private readonly IAmazonS3 _s3Client;

    public JobResultService(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task<APIGatewayProxyResponse> GetResultAsync(
        string jobId)
    {
        var bucketName =
            Environment.GetEnvironmentVariable("DATA_BUCKET_NAME")
            ?? throw new InvalidOperationException(
                "DATA_BUCKET_NAME environment variable is not configured.");

        var objectKey =
            $"results/{jobId}.json";

        try
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey
            };

            using var response =
                await _s3Client.GetObjectAsync(request);

            using var reader =
                new StreamReader(response.ResponseStream);

            var content =
                await reader.ReadToEndAsync();

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = content,
                Headers = CreateCorsHeaders()
            };
        }
        catch (AmazonS3Exception ex)
            when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 202,
                Body = JsonSerializer.Serialize(new
                {
                    jobId,
                    status = "PROCESSING"
                }),
                Headers = CreateCorsHeaders()
            };
        }
    }

    private static Dictionary<string, string>
        CreateCorsHeaders()
    {
        return new Dictionary<string, string>
        {
            ["Content-Type"] = "application/json",
            ["Access-Control-Allow-Origin"] = "*"
        };
    }
}