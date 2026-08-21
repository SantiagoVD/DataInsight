using DataInsight.Worker.Application.Interfaces;
using DataInsight.Worker.Domain.Entities;
using DataInsight.Worker.Domain.Enums;
using DataInsight.Worker.Infrastructure.Storage;
using DataInsight.Worker.Messages;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataInsight.Worker.Application.Services;

public class FileProcessingService : IFileProcessingService
{
    private readonly IS3StorageService _storageService;

    public FileProcessingService(
        IS3StorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<ProcessingResult> ProcessAsync(
        ProcessFileMessage message)
    {
        var csvContent =
            await _storageService.GetFileContentAsync(
                message.BucketName,
                message.ObjectKey);

        var records = ParseCsv(csvContent);

        if (records.Count == 0)
        {
            throw new InvalidOperationException(
                "The CSV file does not contain records.");
        }

        var productMetrics = records
            .GroupBy(record => record.Product)
            .Select(group => new ProductMetric
            {
                Product = group.Key,

                Quantity = group.Sum(
                    record => record.Quantity),

                Revenue = group.Sum(
                    record => record.Quantity * record.Price)
            })
            .OrderByDescending(metric => metric.Revenue)
            .ToList();

        var totalUnits =
            productMetrics.Sum(metric => metric.Quantity);

        var totalRevenue =
            productMetrics.Sum(metric => metric.Revenue);

        var topSellingProduct =
            productMetrics
                .OrderByDescending(metric => metric.Quantity)
                .First()
                .Product;

        var highestRevenueProduct =
            productMetrics
                .OrderByDescending(metric => metric.Revenue)
                .First()
                .Product;

        var result = new ProcessingResult
        {
            JobId = message.JobId,

            FileName = message.FileName,

            Status = ProcessingStatus.Completed,

            TotalRecords = records.Count,

            TotalUnits = totalUnits,

            TotalRevenue = totalRevenue,

            TopSellingProduct = topSellingProduct,

            HighestRevenueProduct =
                highestRevenueProduct,

            Products = productMetrics,

            ProcessedAt = DateTime.UtcNow
        };

        var resultJson =
            JsonSerializer.Serialize(
                result,
                new JsonSerializerOptions
                {
                    WriteIndented = true,

                    Converters =
                    {
                        new JsonStringEnumConverter()
                    }
                });

        var resultKey =
            $"results/{message.JobId}.json";

        await _storageService.SaveResultAsync(
            message.BucketName,
            resultKey,
            resultJson);

        return result;
    }

    private static List<CsvRecord> ParseCsv(
        string csvContent)
    {
        var records = new List<CsvRecord>();

        var lines = csvContent.Split(
            new[]
            {
                "\r\n",
                "\n"
            },
            StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length <= 1)
        {
            return records;
        }

        foreach (var line in lines.Skip(1))
        {
            var columns = line.Split(',');

            if (columns.Length != 3)
            {
                continue;
            }

            var record = new CsvRecord
            {
                Product = columns[0].Trim(),

                Quantity = int.Parse(
                    columns[1],
                    CultureInfo.InvariantCulture),

                Price = decimal.Parse(
                    columns[2],
                    CultureInfo.InvariantCulture)
            };

            records.Add(record);
        }

        return records;
    }
}