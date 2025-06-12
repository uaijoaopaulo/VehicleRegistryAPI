namespace VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws
{
    public interface IAmazonS3Connector
    {
        /// <summary>
        /// Generates a pre-signed URL that allows uploading a file to the specified storage bucket 
        /// without requiring direct authentication credentials. This URL is valid for a limited time.
        /// </summary>
        /// <param name="fileName">The name (including extension) of the file to be uploaded.</param>
        /// <param name="mimeType">The MIME type (e.g., "image/png", "application/pdf") of the file.</param>
        /// <param name="bucketName">The name of the target storage bucket.</param>
        /// <returns>A pre-signed URL that permits uploading the file to the storage service.</returns>
        string GeneratePresignedUrl(string fileName, string mimeType, string bucketName);

        /// <summary>
        /// Generates a temporary, read-only URL to access a specific object stored in the given bucket.
        /// This is typically used to share or preview files securely for a limited duration.
        /// </summary>
        /// <param name="objectKey">The unique key (or path) identifying the object within the storage bucket.</param>
        /// <param name="bucketName">The name of the storage bucket containing the object.</param>
        /// <returns>A time-limited URL that grants read-only access to the specified object.</returns>
        string GetTemporaryAccessUrl(string objectKey, string bucketName);
    }
}
