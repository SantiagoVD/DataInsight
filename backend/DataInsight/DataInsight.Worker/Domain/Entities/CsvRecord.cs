namespace DataInsight.Worker.Domain.Entities;

public class CsvRecord
{
    public string Product { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Price { get; set; }
}