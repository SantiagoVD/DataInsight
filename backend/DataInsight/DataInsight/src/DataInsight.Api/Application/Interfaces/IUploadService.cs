using DataInsight.src.DataInsight.Api.Contracts.Requests;
using DataInsight.src.DataInsight.Api.Contracts.Responses;

namespace DataInsight.src.DataInsight.Api.Application.Interfaces;

public interface IUploadService
{
    Task<CreateUploadResponse> CreateUploadUrlAsync(
        CreateUploadRequest request);
}