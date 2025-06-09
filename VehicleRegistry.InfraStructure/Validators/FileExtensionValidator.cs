using VehicleRegistry.Contracts.InfraStructure.Validators;

namespace VehicleRegistry.InfraStructure.Validators
{
    public class FileExtensionValidator : IFileExtensionValidator
    {
        private static readonly HashSet<string> _allowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".png", ".jpg", ".jpeg"
        };

        private static readonly HashSet<string> _allowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf", "image/png", "image/jpeg"
        };

        public IReadOnlyCollection<string> AllowedExtensions => _allowedExtensions;
        public IReadOnlyCollection<string> AllowedMimeTypes => _allowedMimeTypes;

        public bool IsValidExtension(string extension)
        {
            return _allowedExtensions.Contains(extension);
        }

        public bool IsValidMimeType(string mimeType)
        {
            return _allowedMimeTypes.Contains(mimeType);
        }
    }
}
