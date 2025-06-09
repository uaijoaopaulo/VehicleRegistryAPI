namespace VehicleRegistry.Contracts.InfraStructure.Validators
{
    public interface IFileExtensionValidator
    {
        /// <summary>
        /// Gets the list of allowed file extensions for validation purposes.
        /// </summary>
        IReadOnlyCollection<string> AllowedExtensions { get; }

        /// <summary>
        /// Gets the list of allowed MIME types for validation purposes.
        /// </summary>
        IReadOnlyCollection<string> AllowedMimeTypes { get; }

        /// <summary>
        /// Checks whether the given file extension is allowed.
        /// </summary>
        /// <param name="extension">The file extension to validate (e.g., ".jpg").</param>
        /// <returns><c>true</c> if the extension is allowed; otherwise, <c>false</c>.</returns>
        bool IsValidExtension(string extension);

        /// <summary>
        /// Checks whether the given MIME type is allowed.
        /// </summary>
        /// <param name="mimeType">The MIME type to validate (e.g., "image/jpeg").</param>
        /// <returns><c>true</c> if the MIME type is allowed; otherwise, <c>false</c>.</returns>
        bool IsValidMimeType(string mimeType);
    }
}
