using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using DataInsight.src.DataInsight.Api.Application.Interfaces;
using DataInsight.src.DataInsight.Api.Contracts.Requests;
using System.Text.Json;

namespace DataInsight.src.DataInsight.Api.Functions;

public class CreateJobFunction
{
    private readonly IJobService _jobService;

    public CreateJobFunction(IJobService jobService)
    {
        _jobService = jobService;
    }

    public async Task<APIGatewayProxyResponse> HandleAsync(
        APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Body))
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 400,
                Body = JsonSerializer.Serialize(new
                {
                    message = "Request body is required."
                })
            };
        }

        var createJobRequest =
            JsonSerializer.Deserialize<CreateJobRequest>(
                request.Body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (createJobRequest is null)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 400,
                Body = JsonSerializer.Serialize(new
                {
                    message = "Invalid request body."
                })
            };
        }

        var response =
            await _jobService.CreateJobAsync(createJobRequest);

        return new APIGatewayProxyResponse
        {
            StatusCode = 202,
            Body = JsonSerializer.Serialize(response),
            Headers = new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Access-Control-Allow-Origin"] = "http://localhost:5173"
            }
        };
    }
}