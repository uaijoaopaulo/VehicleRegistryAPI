namespace VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws
{
    public interface IAmazonS3Connector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        string GeneratePresignedUrl(string fileName, string contentType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectKey"></param>
        /// <returns></returns>
        string GetTemporaryAccessUrl(string objectKey);
    }
}
