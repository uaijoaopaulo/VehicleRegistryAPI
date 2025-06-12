using Amazon.S3;
using Amazon.S3.Model;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws;

namespace VehicleRegistry.InfraStructure.AWS
{
    public class AmazonS3Connector(IAmazonS3 amazonS3Client) : IAmazonS3Connector
    {
        private readonly IAmazonS3 _amazonS3Client = amazonS3Client;

        public string GeneratePresignedUrl(string fileName, string mimeType, string bucketName)
        {
            var urlString = string.Empty;
            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = fileName,
                    Verb = HttpVerb.PUT,
                    Expires = DateTime.UtcNow.AddMinutes(15),
                    ContentType = mimeType
                };

                urlString = _amazonS3Client.GetPreSignedURL(request);
            }
            catch (AmazonS3Exception e)
            {
                throw new Exception($"A error occured when try to generate a new presigned URL: {e.Message}");
            }
            return urlString;
        }

        public string GetTemporaryAccessUrl(string objectKey, string bucketName) 
        {
            var urlString = string.Empty;
            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    Verb = HttpVerb.GET,
                    Expires = DateTime.UtcNow.AddHours(1)
                };

                urlString = _amazonS3Client.GetPreSignedURL(request);
            }
            catch (AmazonS3Exception e)
            {
                throw new Exception($"A error occured when try to generate a new presigned URL: {e.Message}");
            }
            return urlString;
        }
    }
}
