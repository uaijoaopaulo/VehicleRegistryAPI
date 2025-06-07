using System.Text.RegularExpressions;
using VehicleRegistry.Contracts.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Interfaces.Manager;
using VehicleRegistry.Contracts.Interfaces.Mongo;

namespace VehicleRegistry.Manager
{
    public class VehiclesManager(IVehiclesRepository vehiclesRepository) : IVehiclesManager
    {
        private readonly IVehiclesRepository _vehiclesRepository = vehiclesRepository;

        public async Task<List<VehicleModel>> GetVehiclesAsync(string? plate, List<string>? plates, int? page = null, int? pageSize = null)
        {
            return await _vehiclesRepository.GetVehiclesAsync(plate, plates, page, pageSize);
        }

        public async Task<VehicleModel> InsertModelAsync(VehicleModel vehicleModel)
        {
            if(vehicleModel.Year < 2020)
            {
                throw new ArgumentException("Year vehicle's should be greater than 2020");
            }

            if (!ValidateLicensePlate(vehicleModel.Plate))
            {
                throw new ArgumentException("Licence plate doens't match with the Brazilian pattern");
            }

            return await _vehiclesRepository.InsertOneAsync(vehicleModel);
        }

        public async Task<VehicleModel> UpdateVehicleAsync(string plate, VehicleModel vehicleModel)
        {
            return await _vehiclesRepository.UpdateVehicleAsync(vehicleModel);
        }

        public async Task DeleteVehicleAsync(string plate)
        {
            await _vehiclesRepository.DeleteVehicleAsync(plate);
        }

        private bool ValidateLicensePlate(string licensePlate)
        {
            if (string.IsNullOrWhiteSpace(licensePlate) || licensePlate.Length < 7)
            {
                return false;
            }

            var regex = new Regex(@"^[A-Za-z]{3}\d{4}$|^[A-Za-z]{3}\d[A-Za-z]\d{2}$");
            return regex.IsMatch(licensePlate);
        }
    }
}
