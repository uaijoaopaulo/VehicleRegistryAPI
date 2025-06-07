namespace VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws
{
    public interface IAmazonS3Connector
    {
        /// <summary>
        /// Generates a pre-signed URL that allows temporary access to upload a file to the specified S3 bucket.
        /// </summary>
        /// <param name="bucketName">The name of the S3 bucket.</param>
        /// <param name="fileName">The name of the file to be uploaded.</param>
        /// <param name="contentType">The MIME type of the file.</param>
        /// <returns>A pre-signed URL for uploading the file.</returns>
        string GeneratePresignedUrl(string bucketName, string fileName, string contentType);
    }
}
