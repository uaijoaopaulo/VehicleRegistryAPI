using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using VehicleRegistry.Api.Controllers;
using VehicleRegistry.Contracts.InfraStructure.Validators;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api.Vehicles;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api;
using VehicleRegistry.Contracts.Interfaces.Manager;
using VehicleRegistry.Contracts.Manager.Vehicle;
using VehicleRegistry.Contracts.Manager.VehicleFiles;

namespace VehicleRegistry.Tests.Unit.Controllers
{
    public class VehiclesControllerTests
    {
        private readonly Mock<ILogger<VehiclesController>> _mockLogger = new();
        private readonly Mock<IVehiclesManager> _mockVehiclesManager = new();
        private readonly Mock<IVehicleFilesManager> _mockVehicleFilesManager = new();
        private readonly Mock<IFileExtensionValidator> _mockFileExtensionValidator = new();

        private readonly VehiclesController _controller;

        public VehiclesControllerTests()
        {
            _controller = new VehiclesController(
                _mockLogger.Object,
                _mockVehiclesManager.Object,
                _mockVehicleFilesManager.Object,
                _mockFileExtensionValidator.Object
            );
        }

        [Fact]
        public async Task GetVehicles_ReturnsOkWithVehicles()
        {
            var vehicles = new List<VehicleDTO>
            {
                new() { Id = 1, Make = "Ford", Model = "Focus", Year = 2020, Plate = "ABC1234" }
            };

            _mockVehiclesManager.Setup(m => m.GetVehiclesAsync(null, null, 1, 50))
                .ReturnsAsync(vehicles);

            var result = await _controller.GetVehicles(null, null, 1, 50);
            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<List<VehicleDTO>>>(ok.Value);
            Assert.NotNull(response.Result);
            Assert.Single(response.Result);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task GetVehicles_OnException_ReturnsBadRequest()
        {
            _mockVehiclesManager.Setup(m => m.GetVehiclesAsync(It.IsAny<string>(), It.IsAny<List<int>>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .ThrowsAsync(new Exception("Database failure"));

            var result = await _controller.GetVehicles(null, null, null, null);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequest.Value);
            Assert.Contains("Database failure", response.Errors);
        }

        [Fact]
        public async Task PostVehicle_ValidVehicle_ReturnsOkWithVehicle()
        {
            var vehicle = new VehicleDTO { Model = "Model S", Year = 2021, Plate = "TESLA123", Make = "Tesla" };
            var insertedVehicle = new VehicleDTO { Id = 100, Model = "Model S", Year = 2021, Plate = "TESLA123", Make = "Tesla" };

            _mockVehiclesManager.Setup(m => m.InsertModelAsync(vehicle)).ReturnsAsync(insertedVehicle);

            var result = await _controller.PostVehicle(vehicle);
            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<VehicleDTO>>(ok.Value);
            Assert.Equal(100, response.Result?.Id);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task PostVehicle_OnException_ReturnsBadRequest()
        {
            var vehicle = new VehicleDTO { Model = "Model X", Year = 2022, Plate = "TESLA999", Make = "Tesla" };

            _mockVehiclesManager.Setup(m => m.InsertModelAsync(It.IsAny<VehicleDTO>()))
                .ThrowsAsync(new Exception("Insert failed"));

            var result = await _controller.PostVehicle(vehicle);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequest.Value);
            Assert.Contains("Insert failed", response.Errors);
        }

        [Fact]
        public async Task DeleteVehicle_Success_ReturnsNoContent()
        {
            _mockVehiclesManager.Setup(m => m.DeleteVehicleAsync(10)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteVehicle(10);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteVehicle_OnException_ReturnsBadRequest()
        {
            _mockVehiclesManager.Setup(m => m.DeleteVehicleAsync(10))
                .ThrowsAsync(new Exception("Delete failed"));

            var result = await _controller.DeleteVehicle(10);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequest.Value);
            Assert.Contains("Delete failed", response.Errors);
        }

        [Fact]
        public async Task PostFileVehicle_ValidFile_ReturnsPresignedUrl()
        {
            var vehicleId = 5;
            var payload = new FileUploadRequest { FileName = "document.pdf", FileMimetype = "application/pdf" };
            var presignedUrl = "https://s3.fakeurl.com/vehicle/5/document.pdf";

            _mockFileExtensionValidator.Setup(v => v.IsValidExtension(".pdf")).Returns(true);
            _mockFileExtensionValidator.Setup(v => v.IsValidMimeType("application/pdf")).Returns(true);

            _mockVehicleFilesManager.Setup(m => m.RegisterVehicleFileAndGetUploadUrlAsync(vehicleId, payload.FileName, payload.FileMimetype))
                .ReturnsAsync(presignedUrl);

            var result = await _controller.PostFileVehicle(vehicleId, payload);
            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<FileUploadResponse>>(ok.Value);
            Assert.Equal(payload.FileName, response.Result?.FileName);
            Assert.Equal(presignedUrl, response.Result?.UploadUrl);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task PostFileVehicle_InvalidExtension_ReturnsBadRequest()
        {
            var vehicleId = 5;
            var payload = new FileUploadRequest { FileName = "malware.exe", FileMimetype = "application/octet-stream" };

            _mockFileExtensionValidator.Setup(v => v.IsValidExtension(".exe")).Returns(false);
            _mockFileExtensionValidator.Setup(v => v.IsValidMimeType("application/octet-stream")).Returns(false);

            var result = await _controller.PostFileVehicle(vehicleId, payload);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequest.Value);
            Assert.Contains("Only .pdf, .png, .jpg, or .jpeg files are allowed.", response.Errors);
            Assert.Contains("Only PDF and image files (PNG, JPG, JPEG) are allowed.", response.Errors);
        }

        [Fact]
        public async Task GetFilesVehicles_ReturnsFileList()
        {
            var vehicleId = 1;

            var files = new List<VehicleFileModel>
            {
                new VehicleFileModel
                {
                    Id = 1,
                    VehicleId = vehicleId,
                    FileName = "document.pdf",
                    FileMimetype = "application/pdf",
                    ObjectKey = "1/document.pdf",
                    Status = FileStatus.Uploaded,
                    CreatedAt = DateTime.UtcNow,
                    GeneratedAt = null
                },
                new VehicleFileModel
                {
                    Id = 2,
                    VehicleId = vehicleId,
                    FileName = "image.jpg",
                    FileMimetype = "image/jpeg",
                    ObjectKey = "1/image.jpg",
                    Status = FileStatus.Uploaded,
                    CreatedAt = DateTime.UtcNow,
                    GeneratedAt = null
                }
            };

            _mockVehicleFilesManager
                .Setup(m => m.GetVehicleFilesAsync(vehicleId))
                .ReturnsAsync(files);

            var result = await _controller.GetFilesVehicles(vehicleId);
            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<List<VehicleFileModel>>>(ok.Value);
            Assert.Equal(2, response.Result?.Count);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task GetFilesVehicles_OnException_ReturnsBadRequest()
        {
            var vehicleId = 3;

            _mockVehicleFilesManager.Setup(m => m.GetVehicleFilesAsync(vehicleId))
                .ThrowsAsync(new Exception("Error fetching files"));

            var result = await _controller.GetFilesVehicles(vehicleId);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequest.Value);
            Assert.Contains("An unexpected error occurred", response.Errors[0]);
        }
    }
}
