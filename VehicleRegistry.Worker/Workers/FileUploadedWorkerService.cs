using VehicleRegistry.Contracts.InfraStructure.AWS.SQS;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws;
using VehicleRegistry.Contracts.Interfaces.Manager;

namespace VehicleRegistry.Worker.Workers
{
    public class FileUploadedWorkerService(IConfiguration configuration, IAmazonSQSConnector amazonSQSConnector, IVehicleFilesManager vehicleFilesManager) : BackgroundService
    {
        private readonly string _queueUrl = configuration["AWSQueueUrl:FileUploaded"]!;
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
                        foreach (var record in fileData.Body!.Records)
                        {
                            if (string.IsNullOrWhiteSpace(record.S3.Object.Key))
                            {
                                continue;
                            }

                            var objectKey = Uri.UnescapeDataString(record.S3.Object.Key);
                            await _vehicleFilesManager.MakeFileAsProcessedAsync(objectKey, record.EventTime ?? DateTime.UtcNow);
                        }
                    }
                }
                catch (Exception)
                {
                    //Log
                }
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
