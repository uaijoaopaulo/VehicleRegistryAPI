namespace VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws
{
    public interface IAmazonS3Connector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        string GeneratePresignedUrl(string fileName, string mimeType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectKey"></param>
        /// <returns></returns>
        string GetTemporaryAccessUrl(string objectKey);
    }
}
