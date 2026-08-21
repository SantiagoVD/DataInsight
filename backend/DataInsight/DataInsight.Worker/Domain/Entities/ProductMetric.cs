namespace DataInsight.Worker.Domain.Entities;

public class ProductMetric
{
    public string Product { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Revenue { get; set; }
}