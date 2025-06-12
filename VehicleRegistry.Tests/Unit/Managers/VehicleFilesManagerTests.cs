using Microsoft.Extensions.Logging;
using Moq;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Manager.VehicleFiles;
using VehicleRegistry.Manager;

namespace VehicleRegistry.Tests.Unit.Managers
{
    public class VehicleFilesManagerTests
    {
        private readonly Mock<IVehicleFilesRepository> _mockRepository = new();
        private readonly Mock<IAmazonS3Connector> _mockS3 = new();
        private readonly Mock<IAmazonConnector> _mockAmazon = new();
        private readonly Mock<ILogger<VehicleFilesManager>> _mockLogger = new();

        private readonly VehicleFilesManager _manager;

        public VehicleFilesManagerTests()
        {
            _manager = new VehicleFilesManager(_mockLogger.Object, _mockRepository.Object, _mockS3.Object, _mockAmazon.Object);
        }

        [Fact]
        public async Task SaveVehicleFileDataAsync_ShouldInsertFile()
        {
            var id = 123;
            var filename = "file.pdf";
            var mimetype = "application/pdf";
            VehicleFileModel? capturedModel = null;

            _mockRepository.Setup(r => r.InsertOneAsync(It.IsAny<VehicleFileModel>()))
                .Callback<VehicleFileModel>(model => capturedModel = model)
                .ReturnsAsync((VehicleFileModel model) => model);

            await _manager.RegisterVehicleFileAndGetUploadUrlAsync(id, filename, mimetype);

            Assert.NotNull(capturedModel);
            Assert.Equal(id, capturedModel!.VehicleId);
            Assert.Equal(filename, capturedModel.FileName);
            Assert.Equal(mimetype, capturedModel.FileMimetype);
            Assert.Equal(FileStatus.Pending, capturedModel.Status);
            Assert.Equal($"{id}/{filename}", capturedModel.ObjectKey);
            _mockRepository.Verify(r => r.InsertOneAsync(It.IsAny<VehicleFileModel>()), Times.Once);
        }

        [Fact]
        public async Task GetVehicleFilesAsync_ShouldReturnFilesWithUrls()
        {
            var vehicleId = 456;
            var files = new List<VehicleFileModel>
            {
                new() { VehicleId = vehicleId, ObjectKey = "456/test.jpg", FileName = "test.jpg" }
            };

            _mockRepository.Setup(r => r.GetVehicleFileAsync(vehicleId)).ReturnsAsync(files);
            _mockS3.Setup(s => s.GetTemporaryAccessUrl("456/test.jpg", It.IsAny<string>())).Returns("https://s3.test/456/test.jpg");

            var result = await _manager.GetVehicleFilesAsync(vehicleId);
            Assert.Single(result);
            Assert.Equal("https://s3.test/456/test.jpg", result.First().FileUrl);
        }

        [Fact]
        public async Task MakeFileAsProcessedAsync_ShouldMarkAsProcessed()
        {
            var objectKey = "789/file.png";
            var eventTime = DateTime.UtcNow;

            _mockRepository.Setup(r => r.MarkFileAsProcessedAsync(objectKey, eventTime)).Returns(Task.CompletedTask);

            await _manager.MarkFileAsProcessedAsync(objectKey, eventTime);
            _mockRepository.Verify(r => r.MarkFileAsProcessedAsync(objectKey, eventTime), Times.Once);
        }

        [Fact]
        public async Task SaveVehicleFileDataAsync_WhenExceptionThrown_ShouldThrowCustomMessage()
        {
            _mockRepository.Setup(r => r.InsertOneAsync(It.IsAny<VehicleFileModel>()))
                .ThrowsAsync(new Exception("Database failure"));

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _manager.RegisterVehicleFileAndGetUploadUrlAsync(1, "file.png", "image/png"));
            Assert.Equal("Error occurred while saving vehicle file.", ex.Message);
        }

        [Fact]
        public async Task GetVehicleFilesAsync_WhenExceptionThrown_ShouldThrowCustomMessage()
        {
            _mockRepository.Setup(r => r.GetVehicleFileAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database error"));

            var ex = await Assert.ThrowsAsync<Exception>(() => _manager.GetVehicleFilesAsync(123));
            Assert.Equal("Error occurred while retrieving vehicle files.", ex.Message);
        }

        [Fact]
        public async Task MakeFileAsProcessedAsync_WhenExceptionThrown_ShouldThrowCustomMessage()
        {
            _mockRepository.Setup(r => r.MarkFileAsProcessedAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ThrowsAsync(new Exception("Update error"));

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _manager.MarkFileAsProcessedAsync("objectKey", DateTime.UtcNow));
            Assert.Equal("Error occurred while updating file status.", ex.Message);
        }
    }
}
