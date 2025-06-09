namespace VehicleRegistry.Contracts.InfraStructure.Validators
{
    public interface IFileExtensionValidator
    {
        /// <summary>
        /// 
        /// </summary>
        IReadOnlyCollection<string> AllowedExtensions { get; }

        /// <summary>
        /// 
        /// </summary>
        IReadOnlyCollection<string> AllowedMimeTypes { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        bool IsValidExtension(string extension);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        bool IsValidMimeType(string mimeType);
    }
}
