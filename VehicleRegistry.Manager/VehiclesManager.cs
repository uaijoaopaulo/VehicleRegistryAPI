using System.Text.RegularExpressions;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Models;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Database;
using VehicleRegistry.Contracts.Interfaces.Manager;

namespace VehicleRegistry.Manager
{
    public class VehiclesManager(IVehiclesRepository vehiclesRepository) : IVehiclesManager
    {
        private readonly IVehiclesRepository _vehiclesRepository = vehiclesRepository;

        public async Task<List<VehicleDTO>> GetVehiclesAsync(string? plate, List<int>? ids, int? page, int? pageSize)
        {
            return await _vehiclesRepository.GetVehiclesAsync(plate, ids, page, pageSize);
        }

        public async Task<VehicleDTO> InsertModelAsync(VehicleDTO vehicleModel)
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

        public async Task<VehicleDTO> UpdateVehicleAsync(VehicleDTO vehicleModel)
        {
            await _vehiclesRepository.UpdateOneAsync(vehicleModel);
            return vehicleModel;
        }

        public async Task DeleteVehicleAsync(int id)
        {
            await _vehiclesRepository.DeleteOneAsync(id);
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
