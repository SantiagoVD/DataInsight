using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using DataInsight.src.DataInsight.Api.Application.Interfaces;
using DataInsight.src.DataInsight.Api.DependencyInjection;
using DataInsight.src.DataInsight.Api.Functions;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(
    typeof(
        Amazon.Lambda.Serialization.SystemTextJson
            .DefaultLambdaJsonSerializer
    )
)]

namespace DataInsight.src.DataInsight.Api;

public class Function
{
    private readonly CreateJobFunction _createJobFunction;
    private readonly CreateUploadFunction _createUploadFunction;
    private readonly GetJobResultFunction _getJobResultFunction;

    public Function()
    {
        var services = new ServiceCollection();

        services.AddApplicationServices();

        var serviceProvider =
            services.BuildServiceProvider();

        var jobService =
            serviceProvider.GetRequiredService<IJobService>();

        var uploadService =
            serviceProvider.GetRequiredService<IUploadService>();

        var jobResultService =
            serviceProvider.GetRequiredService<IJobResultService>();

        _createJobFunction =
            new CreateJobFunction(jobService);

        _createUploadFunction =
            new CreateUploadFunction(uploadService);

        _getJobResultFunction =
            new GetJobResultFunction(jobResultService);
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(
        APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        var method =
            request.HttpMethod?.ToUpperInvariant();

        var path =
            request.Path?.ToLowerInvariant();

        if (method == "POST" &&
            path == "/jobs")
        {
            return await _createJobFunction
                .HandleAsync(request, context);
        }

        if (method == "POST" &&
            path == "/uploads")
        {
            return await _createUploadFunction
                .HandleAsync(request, context);
        }

        if (method == "GET" &&
            path is not null &&
            path.StartsWith("/jobs/"))
        {
            return await _getJobResultFunction
                .HandleAsync(request, context);
        }

        return new APIGatewayProxyResponse
        {
            StatusCode = 404,
            Body = """
            {
              "message": "Route not found."
            }
            """,
            Headers = new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Access-Control-Allow-Origin"] = "*"
            }
        };
    }
}