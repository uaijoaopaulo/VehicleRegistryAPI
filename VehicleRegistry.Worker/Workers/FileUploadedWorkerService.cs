using VehicleRegistry.Contracts.InfraStructure.AWS.SQS;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws;
using VehicleRegistry.Contracts.Interfaces.Manager;

namespace VehicleRegistry.Worker.Workers
{
    public class FileUploadedWorkerService(
        IConfiguration configuration, 
        ILogger<FileUploadedWorkerService> logger, 
        IAmazonSQSConnector amazonSQSConnector, 
        IVehicleFilesManager vehicleFilesManager) : BackgroundService
    {
        private readonly string _queueUrl = configuration["AWSQueueUrl:FileUploaded"]!;
        private readonly ILogger<FileUploadedWorkerService> _logger = logger;
        private readonly IAmazonSQSConnector _amazonSQSConnector = amazonSQSConnector;
        private readonly IVehicleFilesManager _vehicleFilesManager = vehicleFilesManager;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var queueFiles = await _amazonSQSConnector.ReceiveMessagesAsync<S3Message>(_queueUrl, 30);

                    foreach (var fileData in queueFiles)
                    {
                        try
                        {
                            foreach (var record in fileData.Body!.Records)
                            {
                                if (string.IsNullOrWhiteSpace(record.S3.Object.Key))
                                {
                                    continue;
                                }

                                var objectKey = Uri.UnescapeDataString(record.S3.Object.Key);
                                await _vehicleFilesManager.MarkFileAsProcessedAsync(objectKey, record.EventTime ?? DateTime.UtcNow);
                                _logger.LogInformation($"Processed file {objectKey}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing a message from the queue.");
                        }
                        finally
                        {
                            await _amazonSQSConnector.DeleteMessageAsync(fileData, _queueUrl);
                            _logger.LogDebug("Deleted message from queue.");
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An unexpected error occurred while retrieving or iterating messages.");
                }
            }
        }
    }
}
