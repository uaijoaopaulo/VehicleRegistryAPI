using VehicleRegistry.Contracts.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Interfaces.Manager;

namespace VehicleRegistry.Manager
{
    public class VehicleFilesManager(IVehicleFilesRepository vehicleFilesRepository) : IVehicleFilesManager
    {
        private readonly IVehicleFilesRepository _vehicleFilesRepository = vehicleFilesRepository;

        public async Task SaveVehicleFileDataAsync(int idVehicle, string fileName, string contentType)
        {
            try
            {
                var objectKey = $"vehicle/{idVehicle}/{fileName}";

                var newFile = new VehicleFileModel
                {
                    IdVehicle = idVehicle,
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

        public async Task<List<VehicleFileModel>> GetVehicleFilesAsync(string bucketName, int idVehicle)
        {
            try
            {
                var vehicleFiles = await _vehicleFilesRepository.GetVehicleFileAsync(idVehicle);
                var updatedVehicleFiles = vehicleFiles
                    .Select(vehicleFile =>
                    {
                        vehicleFile.FileUrl = $"https://{bucketName}.s3.amazonaws.com/{Uri.EscapeDataString(vehicleFile.ObjectKey)}";
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
