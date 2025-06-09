using VehicleRegistry.InfraStructure.Validators;

namespace VehicleRegistry.Tests.Unit.Validators
{
    public class LicensePlateValidatorTests
    {
        private readonly LicensePlateValidator _validator = new();

        [Theory]
        [InlineData("ABC1234", true)]
        [InlineData("ABC1D23", true)]
        [InlineData("abc1234", true)]
        [InlineData("abc1d23", true)]
        [InlineData("AB12345", false)]
        [InlineData("ABCD123", false)]
        [InlineData("1234ABC", false)]
        [InlineData("ABC123", false)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        [InlineData(null, false)]
        [InlineData("AB@1234", false)]
        public void IsValid_ShouldReturnExpectedResult(string plate, bool expected)
        {
            var result = _validator.IsValid(plate);
            Assert.Equal(expected, result);
        }
    }
}
