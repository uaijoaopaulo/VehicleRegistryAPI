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
    public class VehiclesController(IConfiguration configuration, IVehiclesManager vehiclesManager, IVehicleFilesManager vehicleFilesManager, IAmazonS3Connector amazonS3Connector) : ControllerBase
    {
        private readonly string _bucketName = configuration["S3:VehicleFileBucket"]!;
        private readonly IAmazonS3Connector _amazonS3Connector = amazonS3Connector;
        private readonly IVehiclesManager _vehiclesManager = vehiclesManager;
        private readonly IVehicleFilesManager _vehicleFilesManager = vehicleFilesManager;

        [Authorize(Roles = "vehicle-read")]
        [HttpGet]
        public async Task<IActionResult> GetVehicles([FromQuery] string? plate, [FromQuery] List<string>? ids, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
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

        [Authorize(Roles = "vehicle-admin")]
        [HttpPost]
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

        [Authorize(Roles = "vehicle-admin")]
        [HttpPut("{id}")]
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

        [Authorize(Roles = "vehicle-admin")]
        [HttpDelete("{id}")]
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

        [Authorize(Roles = "vehicle-admin")]
        [HttpPost("{id}/file")]
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

                var fileName = Uri.EscapeDataString(payload.FileName);
                var presignedUrl = _amazonS3Connector.GeneratePresignedUrl(_bucketName, fileName, payload.FileMimetype);
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

        [Authorize(Roles = "vehicle-read")]
        [HttpGet("{id}/file")]
        public async Task<IActionResult> GetFilesVehicles([FromRoute] int id)
        {
            var errors = new List<string>();
            try
            {
                var files = await _vehicleFilesManager.GetVehicleFilesAsync(_bucketName, id);
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
