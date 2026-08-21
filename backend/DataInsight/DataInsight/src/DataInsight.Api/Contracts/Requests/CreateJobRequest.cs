using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInsight.src.DataInsight.Api.Contracts.Requests;

public class CreateJobRequest
{
    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public string ObjectKey { get; set; } = string.Empty;
}