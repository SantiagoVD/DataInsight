using Amazon.Lambda.APIGatewayEvents;

namespace DataInsight.src.DataInsight.Api.Application.Interfaces;

public interface IJobResultService
{
    Task<APIGatewayProxyResponse> GetResultAsync(string jobId);
}