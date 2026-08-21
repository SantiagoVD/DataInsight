using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInsight.src.DataInsight.Api.Infrastructure.Configuration;

public class AwsOptions
{
    public string ProcessingQueueUrl { get; set; } = string.Empty;
}