using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using DataInsight.src.DataInsight.Api.Application.Interfaces;
using System.Text.Json;

namespace DataInsight.src.DataInsight.Api.Functions;

public class GetJobResultFunction
{
    private readonly IJobResultService _jobResultService;

    public GetJobResultFunction(
        IJobResultService jobResultService)
    {
        _jobResultService = jobResultService;
    }

    public async Task<APIGatewayProxyResponse> HandleAsync(
        APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        if (request.PathParameters is null ||
            !request.PathParameters.TryGetValue(
                "jobId",
                out var jobId) ||
            string.IsNullOrWhiteSpace(jobId))
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 400,
                Body = JsonSerializer.Serialize(new
                {
                    message = "jobId is required."
                }),
                Headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json",
                    ["Access-Control-Allow-Origin"] = "*"
                }
            };
        }

        return await _jobResultService
            .GetResultAsync(jobId);
    }
}