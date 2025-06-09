using Microsoft.Extensions.Logging;
using VehicleRegistry.Contracts.InfraStructure.Validators;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Database;
using VehicleRegistry.Contracts.Interfaces.Manager;
using VehicleRegistry.Contracts.Manager.Vehicle;

namespace VehicleRegistry.Manager
{
    public class VehiclesManager(ILogger<VehiclesManager> logger, IVehiclesRepository vehiclesRepository, ILicensePlateValidator licensePlateValidator) : IVehiclesManager
    {
        private readonly ILogger _logger = logger;
        private readonly IVehiclesRepository _vehiclesRepository = vehiclesRepository;
        private readonly ILicensePlateValidator _licensePlateValidator = licensePlateValidator;

        public async Task<List<VehicleDTO>> GetVehiclesAsync(string? plate, List<int>? ids, int? page, int? pageSize)
        {
            _logger.LogInformation($"Fetching vehicles. Plate: {plate}, Ids: {string.Join(",", ids ?? new List<int>())}, Page: {page}, PageSize: {pageSize}");

            var result = await _vehiclesRepository.GetVehiclesAsync(plate, ids, page, pageSize);

            _logger.LogDebug($"Fetched {result.Count} vehicles");

            return result;
        }

        public async Task<VehicleDTO> InsertModelAsync(VehicleDTO vehicleModel)
        {
            _logger.LogInformation($"Inserting new vehicle. Plate: {vehicleModel.Plate}, Year: {vehicleModel.Year}");

            if (vehicleModel.Year < 2020)
            {
                _logger.LogWarning($"Vehicle year {vehicleModel.Year} is less than 2020");
                throw new ArgumentException("Year vehicle's should be greater than 2020");
            }

            if (!_licensePlateValidator.IsValid(vehicleModel.Plate))
            {
                _logger.LogWarning($"Invalid license plate format: {vehicleModel.Plate}");
                throw new ArgumentException("Licence plate doesn't match with the Brazilian pattern");
            }

            var result = await _vehiclesRepository.InsertOneAsync(vehicleModel);

            _logger.LogDebug($"Vehicle inserted successfully. ID: {result.Id}");

            return result;
        }

        public async Task<VehicleDTO> UpdateVehicleAsync(VehicleDTO vehicleModel)
        {
            _logger.LogInformation($"Updating vehicle. ID: {vehicleModel.Id}");

            await _vehiclesRepository.UpdateOneAsync(vehicleModel);

            _logger.LogDebug($"Vehicle updated successfully. ID: {vehicleModel.Id}");

            return vehicleModel;
        }

        public async Task DeleteVehicleAsync(int id)
        {
            _logger.LogInformation($"Deleting vehicle. ID: {id}");

            await _vehiclesRepository.DeleteOneAsync(id);

            _logger.LogDebug($"Vehicle deleted successfully. ID: {id}");
        }
    }
}
