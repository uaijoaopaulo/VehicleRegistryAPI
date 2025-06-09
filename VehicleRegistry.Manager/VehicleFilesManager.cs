using Microsoft.Extensions.Logging;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Interfaces.Manager;
using VehicleRegistry.Contracts.Manager.VehicleFiles;

namespace VehicleRegistry.Manager
{
    public class VehicleFilesManager(ILogger<VehicleFilesManager> logger, IVehicleFilesRepository vehicleFilesRepository, IAmazonS3Connector amazonS3Connector) : IVehicleFilesManager
    {
        private readonly ILogger<VehicleFilesManager> _logger = logger;
        private readonly IVehicleFilesRepository _vehicleFilesRepository = vehicleFilesRepository;
        private readonly IAmazonS3Connector _amazonS3Connector = amazonS3Connector;

        public async Task SaveVehicleFileDataAsync(int vehicleId, string fileName, string mimeType)
        {
            try
            {
                var objectKey = $"{vehicleId}/{fileName}";
                var newFile = new VehicleFileModel
                {
                    VehicleId = vehicleId,
                    FileMimetype = mimeType,
                    FileName = fileName,
                    ObjectKey = objectKey,
                    Status = FileStatus.Pending,
                    GeneratedAt = DateTime.UtcNow
                };

                _logger.LogInformation($"Saving vehicle file data. VehicleId: {vehicleId}, FileName: {fileName}, MimeType: {mimeType}");

                await _vehicleFilesRepository.InsertOneAsync(newFile);

                _logger.LogDebug($"Vehicle file data saved successfully. ObjectKey: {objectKey}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while saving vehicle file. VehicleId: {vehicleId}, FileName: {fileName}");
                throw new Exception("Error occurred while saving vehicle file.");
            }
        }

        public async Task<List<VehicleFileModel>> GetVehicleFilesAsync(int vehicleId)
        {
            try
            {
                _logger.LogInformation($"Retrieving vehicle files. VehicleId: {vehicleId}");

                var vehicleFiles = await _vehicleFilesRepository.GetVehicleFileAsync(vehicleId);
                var updatedVehicleFiles = vehicleFiles
                    .Select(vehicleFile =>
                    {
                        vehicleFile.FileUrl = _amazonS3Connector.GetTemporaryAccessUrl(vehicleFile.ObjectKey);
                        return vehicleFile;
                    })
                    .ToList();

                _logger.LogDebug($"Retrieved {updatedVehicleFiles.Count} vehicle files. VehicleId: {vehicleId}");

                return updatedVehicleFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving vehicle files. VehicleId: {vehicleId}");
                throw new Exception("Error occurred while retrieving vehicle files.");
            }
        }

        public async Task MakeFileAsProcessedAsync(string objectKey, DateTime eventTime)
        {
            try
            {
                _logger.LogInformation($"Marking file as processed. ObjectKey: {objectKey}, EventTime: {eventTime}");

                await _vehicleFilesRepository.MarkFileAsProcessedAsync(objectKey, eventTime);

                _logger.LogDebug($"File marked as processed successfully. ObjectKey: {objectKey}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating file status. ObjectKey: {objectKey}");
                throw new Exception("Error occurred while updating file status.");
            }
        }
    }
}