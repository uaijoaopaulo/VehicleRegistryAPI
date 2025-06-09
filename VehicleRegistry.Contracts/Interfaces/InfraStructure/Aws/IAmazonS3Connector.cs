namespace VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws
{
    public interface IAmazonS3Connector
    {
        /// <summary>
        /// Generates a pre-signed URL that allows uploading a file to the storage service 
        /// without requiring authentication.
        /// </summary>
        /// <param name="fileName">The name of the file to be uploaded.</param>
        /// <param name="mimeType">The MIME type of the file.</param>
        /// <returns>A pre-signed URL for uploading the file.</returns>
        string GeneratePresignedUrl(string fileName, string mimeType);

        /// <summary>
        /// Retrieves a temporary URL that provides read-only access to a stored object.
        /// </summary>
        /// <param name="objectKey">The key (or name) of the object in the storage.</param>
        /// <returns>A temporary access URL for the specified object.</returns>
        string GetTemporaryAccessUrl(string objectKey);
    }
}
