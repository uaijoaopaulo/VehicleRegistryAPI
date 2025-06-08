using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRegistry.Contracts.InfraStructure.Mongo;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api.Vehicles;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Models;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws;
using VehicleRegistry.Contracts.Interfaces.Manager;

namespace VehicleRegistry.Api.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    public class VehiclesController(IVehiclesManager vehiclesManager, IVehicleFilesManager vehicleFilesManager, IAmazonS3Connector amazonS3Connector) : ControllerBase
    {
        private readonly IAmazonS3Connector _amazonS3Connector = amazonS3Connector;
        private readonly IVehiclesManager _vehiclesManager = vehiclesManager;
        private readonly IVehicleFilesManager _vehicleFilesManager = vehicleFilesManager;

        [HttpGet]
        [Authorize(Roles = "vehicle-read")]
        public async Task<IActionResult> GetVehicles([FromQuery] string? plate, [FromQuery] List<int>? ids, [FromQuery] int? page = 1, [FromQuery] int? pageSize = 50)
        {
            var errors = new List<string>();
            try
            {
                var allVehicles = await _vehiclesManager.GetVehiclesAsync(plate, ids, page, pageSize);
                return Ok(new ApiResponse<List<VehicleDTO>>
                {
                    Result = allVehicles
                });
            }
            catch (Exception e)
            {
                errors.Add(e.Message);
                return BadRequest(new ApiResponse<VehicleDTO>
                {
                    Errors = errors
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "vehicle-admin")]
        public async Task<IActionResult> PostVehicle([FromBody] VehicleDTO vehicleModel)
        {
            var errors = new List<string>();
            try
            {
                var vehicleResponse = await _vehiclesManager.InsertModelAsync(vehicleModel);
                return Ok(new ApiResponse<VehicleDTO>
                {
                    Result = vehicleResponse
                });
            }
            catch (Exception e)
            {
                errors.Add(e.Message);
                return BadRequest(new ApiResponse<VehicleDTO>
                {
                    Errors = errors
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "vehicle-admin")]
        public async Task<IActionResult> PutVehicle([FromRoute] int id, [FromBody] VehicleDTO vehicleModel)
        {
            var errors = new List<string>();
            try
            {
                vehicleModel.Id = id;
                var vehicleResponse = await _vehiclesManager.UpdateVehicleAsync(vehicleModel);
                return Ok(new ApiResponse<VehicleDTO>
                {
                    Result = vehicleResponse
                });
            }
            catch (Exception e)
            {
                errors.Add(e.Message);
                return BadRequest(new ApiResponse<VehicleDTO>
                {
                    Errors = errors
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "vehicle-admin")]
        public async Task<IActionResult> DeleteVehicle([FromRoute] int id)
        {
            var errors = new List<string>();
            try
            {
                await _vehiclesManager.DeleteVehicleAsync(id);
                return NoContent();
            }
            catch (Exception e)
            {
                errors.Add(e.Message);
                return BadRequest(new ApiResponse<VehicleDTO>
                {
                    Errors = errors
                });
            }
        }

        [HttpPost("{id}/file")]
        [Authorize(Roles = "vehicle-admin")]
        public async Task<IActionResult> PostFileVehicle([FromRoute] int id, [FromBody] FileUploadRequest payload)
        {
            var errors = new List<string>();
            try
            {
                var allowedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg" };
                var fileExtension = Path.GetExtension(payload.FileName)?.ToLowerInvariant();
                if (!string.IsNullOrEmpty(fileExtension) && !allowedExtensions.Contains(fileExtension))
                {
                    errors.Add("Only .pdf, .png, .jpg, or .jpeg files are allowed.");
                }

                var allowedMimetypes = new[] { "application/pdf", "image/png", "image/jpeg" };
                if (!allowedMimetypes.Contains(payload.FileMimetype.ToLowerInvariant()))
                {
                    errors.Add("Only PDF and image files (PNG, JPG, JPEG) are allowed.");
                }

                if (errors.Any())
                {
                    return BadRequest(new ApiResponse<VehicleFileModel>
                    {
                        Errors = errors
                    });
                }

                await _vehicleFilesManager.SaveVehicleFileDataAsync(id, payload.FileName, payload.FileMimetype);

                var fileName = $"{id}/{Uri.EscapeDataString(payload.FileName)}";
                var presignedUrl = _amazonS3Connector.GeneratePresignedUrl(fileName, payload.FileMimetype);
                var response = new FileUploadResponse
                {
                    FileName = payload.FileName,
                    FileMimetype = payload.FileMimetype,
                    UploadUrl = presignedUrl
                };

                return Ok(new ApiResponse<FileUploadResponse>
                {
                    Result = response
                });
            }
            catch (Exception e)
            {
                errors.Add(e.Message);
                return BadRequest(new ApiResponse<FileUploadResponse>
                {
                    Errors = errors
                });
            }
        }

        [HttpGet("{id}/file")]
        [Authorize(Roles = "vehicle-read")]
        public async Task<IActionResult> GetFilesVehicles([FromRoute] int id)
        {
            var errors = new List<string>();
            try
            {
                var files = await _vehicleFilesManager.GetVehicleFilesAsync(id);
                return Ok(new ApiResponse<List<VehicleFileModel>>
                {
                    Result = files
                });
            }
            catch (Exception e)
            {
                errors.Add(e.Message);
                return BadRequest(new ApiResponse<VehicleFileModel>
                {
                    Errors = errors
                });
            }
        }
    }
}
