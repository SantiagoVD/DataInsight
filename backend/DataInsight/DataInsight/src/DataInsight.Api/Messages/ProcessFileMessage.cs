using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInsight.src.DataInsight.Api.Messages
{
    public class ProcessFileMessage
    {
        public string JobId { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public string BucketName { get; set; } = string.Empty;

        public string ObjectKey { get; set; } = string.Empty;
    }
}
