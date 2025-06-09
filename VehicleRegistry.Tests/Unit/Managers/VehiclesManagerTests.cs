using Microsoft.Extensions.Logging;
using Moq;
using VehicleRegistry.Contracts.InfraStructure.Validators;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Database;
using VehicleRegistry.Contracts.Manager.Vehicle;
using VehicleRegistry.Manager;

namespace VehicleRegistry.Tests.Unit.Managers
{
    public class VehiclesManagerTests
    {
        private readonly Mock<ILogger<VehiclesManager>> _loggerMock;
        private readonly Mock<IVehiclesRepository> _repositoryMock;
        private readonly Mock<ILicensePlateValidator> _plateValidatorMock;
        private readonly VehiclesManager _vehiclesManager;

        public VehiclesManagerTests()
        {
            _loggerMock = new Mock<ILogger<VehiclesManager>>();
            _repositoryMock = new Mock<IVehiclesRepository>();
            _plateValidatorMock = new Mock<ILicensePlateValidator>();

            _vehiclesManager = new VehiclesManager(
                _loggerMock.Object,
                _repositoryMock.Object,
                _plateValidatorMock.Object);
        }

        [Fact]
        public async Task GetVehiclesAsync_ShouldReturnListFromRepository()
        {
            var expectedList = new List<VehicleDTO>
            {
                new VehicleDTO { Id = 1, Make = "Ford", Model = "Focus", Year = 2021, Plate = "ABC1234" },
                new VehicleDTO { Id = 2, Make = "Toyota", Model = "Corolla", Year = 2022, Plate = "XYZ5678" }
            };
            _repositoryMock.Setup(r => r.GetVehiclesAsync("ABC", null, 1, 10))
                .ReturnsAsync(expectedList);

            var result = await _vehiclesManager.GetVehiclesAsync("ABC", null, 1, 10);

            Assert.Equal(expectedList.Count, result.Count);
            Assert.Equal(expectedList.First().Id, result.First().Id);
            _repositoryMock.Verify(r => r.GetVehiclesAsync("ABC", null, 1, 10), Times.Once);
        }

        [Fact]
        public async Task InsertModelAsync_ShouldInsert_WhenDataIsValid()
        {
            var vehicle = new VehicleDTO { Id = 0, Make = "Honda", Model = "Civic", Year = 2023, Plate = "DEF1234" };
            _plateValidatorMock.Setup(v => v.IsValid(vehicle.Plate)).Returns(true);
            _repositoryMock.Setup(r => r.InsertOneAsync(vehicle)).ReturnsAsync(() =>
            {
                vehicle.Id = 10;
                return vehicle;
            });

            var result = await _vehiclesManager.InsertModelAsync(vehicle);

            Assert.Equal(10, result.Id);
            _plateValidatorMock.Verify(v => v.IsValid(vehicle.Plate), Times.Once);
            _repositoryMock.Verify(r => r.InsertOneAsync(vehicle), Times.Once);
        }

        [Fact]
        public async Task InsertModelAsync_ShouldThrowArgumentException_WhenYearIsInvalid()
        {
            var vehicle = new VehicleDTO { Year = 2019, Plate = "AAA1111", Model = "ModelX" };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _vehiclesManager.InsertModelAsync(vehicle));
            Assert.Equal("Year vehicle's should be greater than 2020", ex.Message);
            _plateValidatorMock.Verify(v => v.IsValid(It.IsAny<string>()), Times.Never);
            _repositoryMock.Verify(r => r.InsertOneAsync(It.IsAny<VehicleDTO>()), Times.Never);
        }

        [Fact]
        public async Task InsertModelAsync_ShouldThrowArgumentException_WhenPlateIsInvalid()
        {
            var vehicle = new VehicleDTO { Year = 2022, Plate = "INVALID", Model = "ModelY" };
            _plateValidatorMock.Setup(v => v.IsValid(vehicle.Plate)).Returns(false);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _vehiclesManager.InsertModelAsync(vehicle));
            Assert.Equal("Licence plate doesn't match with the Brazilian pattern", ex.Message);
            _plateValidatorMock.Verify(v => v.IsValid(vehicle.Plate), Times.Once);
            _repositoryMock.Verify(r => r.InsertOneAsync(It.IsAny<VehicleDTO>()), Times.Never);
        }

        [Fact]
        public async Task UpdateVehicleAsync_ShouldCallRepositoryUpdate()
        {
            var vehicle = new VehicleDTO { Id = 5, Year = 2022, Plate = "BBB2222", Model = "ModelZ" };

            _repositoryMock.Setup(r => r.UpdateOneAsync(vehicle))
                .ReturnsAsync(vehicle);

            var result = await _vehiclesManager.UpdateVehicleAsync(vehicle);
            Assert.Equal(vehicle, result);
            _repositoryMock.Verify(r => r.UpdateOneAsync(vehicle), Times.Once);
        }

        [Fact]
        public async Task DeleteVehicleAsync_ShouldCallRepositoryDelete()
        {
            var vehicleId = 3;
            _repositoryMock.Setup(r => r.DeleteOneAsync(vehicleId)).Returns(Task.CompletedTask);

            await _vehiclesManager.DeleteVehicleAsync(vehicleId);
            _repositoryMock.Verify(r => r.DeleteOneAsync(vehicleId), Times.Once);
        }
    }
}