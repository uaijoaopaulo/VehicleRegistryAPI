using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRegistry.Contracts.InfraStructure.Mongo;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api;
using VehicleRegistry.Contracts.Interfaces.Manager;

namespace VehicleRegistry.Api.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    public class VehiclesController(IVehiclesManager vehiclesManager, IVehicleFilesManager vehicleFilesManager) : ControllerBase
    {
        private readonly IVehiclesManager _vehiclesManager = vehiclesManager;
        private readonly IVehicleFilesManager _vehicleFilesManager = vehicleFilesManager;

        [Authorize(Roles = "vehicle-read")]
        [HttpGet]
        public async Task<IActionResult> GetVehicles([FromQuery] string? plate, [FromQuery] List<string>? ids, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            try
            {
                var allVehicles = await _vehiclesManager.GetVehiclesAsync(plate, ids, page, pageSize);
                return Ok(new ApiResponse<List<VehicleModel>>
                {
                    Result = allVehicles
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse<VehicleModel>
                {
                    Errors = new List<string> { e.Message }
                });
            }
        }

        [Authorize(Roles = "vehicle-admin")]
        [HttpPost]
        public async Task<IActionResult> PostVehicle([FromBody] VehicleModel vehicleModel)
        {
            try
            {
                var vehicleResponse = await _vehiclesManager.InsertModelAsync(vehicleModel);
                return Ok(new ApiResponse<VehicleModel>
                {
                    Result = vehicleResponse
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse<VehicleModel>
                {
                    Errors = new List<string> { e.Message }
                });
            }
        }

        [Authorize(Roles = "vehicle-admin")]
        [HttpPut("{plate}")]
        public async Task<IActionResult> PutVehicle(string plate, [FromBody] VehicleModel vehicleModel)
        {
            try
            {
                var vehicleResponse = await _vehiclesManager.UpdateVehicleAsync(plate, vehicleModel);
                return Ok(new ApiResponse<VehicleModel>
                {
                    Result = vehicleResponse
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse<VehicleModel>
                {
                    Errors = new List<string> { e.Message }
                });
            }
        }

        [Authorize(Roles = "vehicle-admin")]
        [HttpDelete("{plate}")]
        public async Task<IActionResult> DeleteVehicle(string plate)
        {
            try
            {
                await _vehiclesManager.DeleteVehicleAsync(plate);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse<VehicleModel>
                {
                    Errors = new List<string> { e.Message }
                });
            }
        }

        [Authorize(Roles = "vehicle-admin")]
        [HttpPost("{plate}/file")]
        public async Task<IActionResult> PostFileVehicle([FromRoute] string plate, [FromBody] FileUploadRequest payload)
        {
            try
            {
                if (!Request.HasFormContentType || Request.Form.Files.Count == 0)
                {
                    throw new ArgumentNullException("No files were uploaded.");
                }

                var presignedUrl = await _vehicleFilesManager.GeneratePresignedUrl(plate, payload.FileName, payload.FileMimetype);

                var response = new FileUploadResponse
                {
                    FileName = payload.FileName,
                    FileMimetype = payload.FileMimetype,
                    UploadUrl = presignedUrl
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse<VehicleFileModel>
                {
                    Errors = new List<string> { e.Message }
                });
            }
        }

        [Authorize(Roles = "vehicle-read")]
        [HttpGet("{plate}/file")]
        public async Task<IActionResult> GetFilesVehicles([FromRoute] string plate)
        {
            try
            {
                var files = await _vehicleFilesManager.GetVehicleFilesAsync(plate);

                return Ok(new ApiResponse<List<VehicleFileModel>>
                {
                    Result = files
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse<VehicleFileModel>
                {
                    Errors = new List<string> { e.Message }
                });
            }
        }
    }
}
