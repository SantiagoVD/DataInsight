using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInsight.src.DataInsight.Api.Contracts.Responses
{
    public class CreateJobResponse
    {
        public string JobId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
