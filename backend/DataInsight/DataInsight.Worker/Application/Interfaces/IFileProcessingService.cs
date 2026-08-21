using DataInsight.Worker.Domain.Entities;
using DataInsight.Worker.Messages;

namespace DataInsight.Worker.Application.Interfaces;

public interface IFileProcessingService
{
    Task<ProcessingResult> ProcessAsync(
        ProcessFileMessage message);
}