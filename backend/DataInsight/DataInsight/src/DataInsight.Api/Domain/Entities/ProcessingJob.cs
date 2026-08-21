using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using DataInsight.src.DataInsight.Api.Domain.Enums;

namespace DataInsight.src.DataInsight.Api.Domain.Entities;
public class ProcessingJob
{
    public string JobId { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public JobStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
}
