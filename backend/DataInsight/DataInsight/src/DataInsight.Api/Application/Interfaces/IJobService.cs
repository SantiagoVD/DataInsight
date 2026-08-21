using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataInsight.src.DataInsight.Api.Contracts.Requests;
using DataInsight.src.DataInsight.Api.Contracts.Responses;

namespace DataInsight.src.DataInsight.Api.Application.Interfaces
{
    public interface IJobService
    {
        Task<CreateJobResponse> CreateJobAsync (CreateJobRequest request);
    }
}
