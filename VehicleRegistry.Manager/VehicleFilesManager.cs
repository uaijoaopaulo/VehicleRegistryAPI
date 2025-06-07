using Microsoft.Extensions.Configuration;
using VehicleRegistry.Contracts.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws;
using VehicleRegistry.Contracts.Interfaces.Manager;
using VehicleRegistry.Contracts.Interfaces.Mongo;

namespace VehicleRegistry.Manager
{
    public class VehicleFilesManager(IConfiguration configuration, IVehicleFilesRepository vehicleFilesRepository, IAmazonS3Connector amazonS3Connector) : IVehicleFilesManager
    {
        private readonly string _bucketName = configuration["S3:VehicleFileBucket"]!;
        private readonly IVehicleFilesRepository _vehicleFilesRepository = vehicleFilesRepository;
        private readonly IAmazonS3Connector _amazonS3Connector = amazonS3Connector;

        public async Task<string> GeneratePresignedUrl(string vehiclePlate, string fileName, string contentType)
        {
            try
            {
                var objectKey = $"veiculos/{vehiclePlate}/{fileName}";

                var newFile = new VehicleFileModel
                {
                    vehiclePlate = vehiclePlate,
                    FileMimetype = contentType,
                    FileName = fileName,
                    ObjectKey = objectKey,
                    Status = FileStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _vehicleFilesRepository.InsertOneAsync(newFile);

                return _amazonS3Connector.GeneratePresignedUrl(_bucketName, fileName, contentType);
            }
            catch (Exception)
            {
                throw new Exception("Error occurred generating the presigned URL.");
            }
        }

        public async Task<List<VehicleFileModel>> GetVehicleFilesAsync(string plate)
        {
            try
            {
                return await _vehicleFilesRepository.GetVehicleFileAsync(plate);
            }
            catch (Exception)
            {
                throw new Exception("Error occurred getting the files");
            }
        }
    }
}
