using Microsoft.Extensions.Configuration;
using VehicleRegistry.Contracts.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Interfaces.Manager;

namespace VehicleRegistry.Manager
{
    public class VehicleFilesManager(IConfiguration configuration, IVehicleFilesRepository vehicleFilesRepository, IAmazonS3Connector amazonS3Connector) : IVehicleFilesManager
    {
        private readonly string _awsRegion = configuration["AWS:Region"]!;
        private readonly string _bucketName = configuration["S3:VehicleFileBucket"]!;
        private readonly IVehicleFilesRepository _vehicleFilesRepository = vehicleFilesRepository;
        private readonly IAmazonS3Connector _amazonS3Connector = amazonS3Connector;

        public async Task SaveVehicleFileDataAsync(int idVehicle, string fileName, string contentType)
        {
            try
            {
                var objectKey = $"{idVehicle}/{fileName}";
                var newFile = new VehicleFileModel
                {
                    VehicleId = idVehicle,
                    FileMimetype = contentType,
                    FileName = fileName,
                    ObjectKey = objectKey,
                    Status = FileStatus.Pending,
                    GeneratedAt = DateTime.UtcNow
                };

                await _vehicleFilesRepository.InsertOneAsync(newFile);
            }
            catch (Exception)
            {
                throw new Exception("Error occurred generating the presigned URL.");
            }
        }

        public async Task<List<VehicleFileModel>> GetVehicleFilesAsync(int idVehicle)
        {
            try
            {
                var vehicleFiles = await _vehicleFilesRepository.GetVehicleFileAsync(idVehicle);
                var updatedVehicleFiles = vehicleFiles
                    .Select(vehicleFile =>
                    {
                        vehicleFile.FileUrl = _amazonS3Connector.GetTemporaryAccessUrl(vehicleFile.ObjectKey);
                        return vehicleFile;
                    })
                    .ToList();

                return updatedVehicleFiles;
            }
            catch (Exception)
            {
                throw new Exception("Error occurred getting the files");
            }
        }

        public async Task MakeFileAsProcessedAsync(string objectKey, DateTime eventTime)
        {
            try
            {
                await _vehicleFilesRepository.MarkFileAsProcessedAsync(objectKey, eventTime);
            }
            catch (Exception)
            {
                throw new Exception("Error occurred processing the update file's status");
            }
        }
    }
}