using DataInsight.Worker.Domain.Enums;

namespace DataInsight.Worker.Domain.Entities;

public class ProcessingResult
{
    public string JobId { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public ProcessingStatus Status { get; set; }

    public int TotalRecords { get; set; }

    public int TotalUnits { get; set; }

    public decimal TotalRevenue { get; set; }

    public string TopSellingProduct { get; set; } = string.Empty;

    public string HighestRevenueProduct { get; set; } = string.Empty;

    public List<ProductMetric> Products { get; set; } = new();

    public DateTime ProcessedAt { get; set; }
}