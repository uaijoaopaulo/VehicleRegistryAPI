using VehicleRegistry.InfraStructure.Validators;

namespace VehicleRegistry.Tests.Unit.Validators
{
    public class FileExtensionValidatorTests
    {
        private readonly FileExtensionValidator _validator = new();

        [Theory]
        [InlineData(".pdf", true)]
        [InlineData(".png", true)]
        [InlineData(".jpg", true)]
        [InlineData(".jpeg", true)]
        [InlineData(".txt", false)]
        [InlineData(".exe", false)]
        public void IsValidExtension_ShouldReturnExpectedResult(string extension, bool expected)
        {
            var result = _validator.IsValidExtension(extension);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("application/pdf", true)]
        [InlineData("image/png", true)]
        [InlineData("image/jpeg", true)]
        [InlineData("application/zip", false)]
        [InlineData("text/plain", false)]
        public void IsValidMimeType_ShouldReturnExpectedResult(string mimeType, bool expected)
        {
            var result = _validator.IsValidMimeType(mimeType);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void AllowedExtensions_ShouldContainAllExpectedValues()
        {
            var expected = new[] { ".pdf", ".png", ".jpg", ".jpeg" };
            Assert.All(expected, e => Assert.Contains(e, _validator.AllowedExtensions));
        }

        [Fact]
        public void AllowedMimeTypes_ShouldContainAllExpectedValues()
        {
            var expected = new[] { "application/pdf", "image/png", "image/jpeg" };
            Assert.All(expected, m => Assert.Contains(m, _validator.AllowedMimeTypes));
        }
    }
}
