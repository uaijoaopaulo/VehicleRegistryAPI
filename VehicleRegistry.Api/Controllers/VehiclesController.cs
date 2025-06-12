using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRegistry.Contracts.InfraStructure.Validators;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api.Vehicles;
using VehicleRegistry.Contracts.Interfaces.Manager;
using VehicleRegistry.Contracts.Manager.Vehicle;

namespace VehicleRegistry.Api.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    public class VehiclesController(ILogger<VehiclesController> logger, IVehiclesManager vehiclesManager, IVehicleFilesManager vehicleFilesManager, IFileExtensionValidator fileExtensionValidator) : ControllerBase
    {
        private readonly ILogger<VehiclesController> _logger = logger;
        private readonly IVehiclesManager _vehiclesManager = vehiclesManager;
        private readonly IVehicleFilesManager _vehicleFilesManager = vehicleFilesManager;
        private readonly IFileExtensionValidator _fileExtensionValidator = fileExtensionValidator;

        [HttpGet]
        [Authorize(Roles = "vehicle-read")]
        public async Task<IActionResult> GetVehicles([FromQuery] string? plate, [FromQuery] List<int>? vehicleIds, [FromQuery] int? page = 1, [FromQuery] int? pageSize = 50)
        {
            var errors = new List<string>();

            _logger.LogInformation("Request received to get vehicles. Plate: {Plate}, Ids: {Ids}, Page: {Page}, PageSize: {PageSize}",
                plate, string.Join(",", vehicleIds ?? new List<int>()), page, pageSize);

            try
            {
                var response = await _vehiclesManager.GetVehiclesAsync(plate, vehicleIds, page, pageSize);

                _logger.LogDebug($"Vehicles query return {response.Count} results.");

                return Ok(ApiResponseHelper.Success(response));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while retrieving vehicles. Plate: {Plate}, Ids: {Ids}, Page: {Page}, PageSize: {PageSize}",
                    plate, string.Join(",", vehicleIds ?? new List<int>()), page, pageSize);

                errors.Add(e.Message);
                return BadRequest(ApiResponseHelper.Failure(errors));
            }
        }

        [HttpPost]
        [Authorize(Roles = "vehicle-admin")]
        public async Task<IActionResult> PostVehicle([FromBody] VehicleDTO vehicleModel)
        {
            var errors = new List<string>();

            _logger.LogInformation("Request received to create a new vehicle. Plate: {Plate}, Model: {Model}, Year: {Year}, Make: {Make}",
                vehicleModel.Plate, vehicleModel.Model, vehicleModel.Year, vehicleModel.Make ?? string.Empty);

            try
            {
                var response = await _vehiclesManager.InsertModelAsync(vehicleModel);

                _logger.LogDebug($"Vehicle successfully created. ID: {response.Id}");

                return Ok(ApiResponseHelper.Success(response));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while creating a new vehicle. Plate: {vehicleModel.Plate}");

                errors.Add(e.Message);
                return BadRequest(ApiResponseHelper.Failure(errors));
            }
        }

        [HttpPut("{vehicleId}")]
        [Authorize(Roles = "vehicle-admin")]
        public async Task<IActionResult> PutVehicle([FromRoute] int vehicleId, [FromBody] VehicleDTO vehicleModel)
        {
            var errors = new List<string>();

            _logger.LogInformation("Request received to update vehicle. ID: {Id}, Plate: {Plate}, Model: {Model}, Year: {Year}, Make: {Make}",
                vehicleId, vehicleModel.Plate, vehicleModel.Model, vehicleModel.Year, vehicleModel.Make ?? string.Empty);

            try
            {
                vehicleModel.Id = vehicleId;
                var response = await _vehiclesManager.UpdateVehicleAsync(vehicleModel);

                _logger.LogDebug($"Vehicle successfully updated. ID: {response.Id}");

                return Ok(ApiResponseHelper.Success(response));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while updating vehicle. ID: {vehicleId}");

                errors.Add(e.Message);
                return BadRequest(ApiResponseHelper.Failure(errors));
            }
        }

        [HttpDelete("{vehicleId}")]
        [Authorize(Roles = "vehicle-admin")]
        public async Task<IActionResult> DeleteVehicle([FromRoute] int vehicleId)
        {
            var errors = new List<string>();
            try
            {
                await _vehiclesManager.DeleteVehicleAsync(vehicleId);

                _logger.LogDebug("Vehicle successfully deleted. ID: {Id}", vehicleId);

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected error while deleting vehicle. ID: {vehicleId}");
                errors.Add(e.Message);
                return BadRequest(ApiResponseHelper.Failure(errors));
            }
        }

        [HttpPost("{vehicleId}/file")]
        [Authorize(Roles = "vehicle-admin")]
        public async Task<IActionResult> PostFileVehicle([FromRoute] int vehicleId, [FromBody] FileUploadRequest payload)
        {
            var errors = new List<string>();

            _logger.LogInformation($"Request received to upload a file for vehicle. ID: {vehicleId}, FileName: {payload.FileName}, MimeType: {payload.FileMimetype}");

            try
            {
                var fileExtension = Path.GetExtension(payload.FileName)?.ToLowerInvariant();
                if (!string.IsNullOrEmpty(fileExtension) && !_fileExtensionValidator.IsValidExtension(fileExtension))
                {
                    errors.Add("Only .pdf, .png, .jpg, or .jpeg files are allowed.");
                }

                if (!_fileExtensionValidator.IsValidMimeType(payload.FileMimetype))
                {
                    errors.Add("Only PDF and image files (PNG, JPG, JPEG) are allowed.");
                }

                if (errors.Any())
                {
                    _logger.LogWarning($"File upload validation failed for ID: {vehicleId}. Errors: {string.Join("; ", errors)}");
                    return BadRequest(ApiResponseHelper.Failure(errors));
                }

                var presignedUrl = await _vehicleFilesManager.RegisterVehicleFileAndGetUploadUrlAsync(vehicleId, payload.FileName, payload.FileMimetype);

                _logger.LogDebug($"Presigned URL generated successfully for ID: {vehicleId}, FileName: {payload.FileName}");

                var response = new FileUploadResponse
                {
                    FileName = payload.FileName,
                    FileMimetype = payload.FileMimetype,
                    UploadUrl = presignedUrl
                };
                return Ok(ApiResponseHelper.Success(response));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected error while processing file upload for ID: {vehicleId}");
                errors.Add("An unexpected error occurred. Please try again later.");
                return BadRequest(ApiResponseHelper.Failure(errors));
            }
        }

        [HttpGet("{vehicleId}/file")]
        [Authorize(Roles = "vehicle-read")]
        public async Task<IActionResult> GetFilesVehicles([FromRoute] int vehicleId)
        {
            var errors = new List<string>();
            try
            {
                var response = await _vehicleFilesManager.GetVehicleFilesAsync(vehicleId);

                _logger.LogDebug($"Retrieved {response.Count} files for ID: {vehicleId}");

                return Ok(ApiResponseHelper.Success(response));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error while getting files upload for ID: {VehicleId}", vehicleId);
                errors.Add("An unexpected error occurred. Please try again later.");
                return BadRequest(ApiResponseHelper.Failure(errors));
            }
        }
    }
}