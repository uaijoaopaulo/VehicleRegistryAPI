using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws;

namespace VehicleRegistry.InfraStructure.AWS
{
    public class AmazonS3Connector(IConfiguration configuration, IAmazonS3 amazonS3Client) : IAmazonS3Connector
    {
        private readonly string _bucketName = configuration["S3:VehicleFileBucket"]!;
        private readonly IAmazonS3 _amazonS3Client = amazonS3Client;

        public string GeneratePresignedUrl(string fileName, string contentType)
        {
            var urlString = string.Empty;
            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = fileName,
                    Verb = HttpVerb.PUT,
                    Expires = DateTime.UtcNow.AddMinutes(15),
                    ContentType = contentType
                };

                urlString = _amazonS3Client.GetPreSignedURL(request);
            }
            catch (AmazonS3Exception e)
            {
                throw new Exception($"A error occured when try to generate a new presigned URL: {e.Message}");
            }
            return urlString;
        }

        public string GetTemporaryAccessUrl(string objectKey) 
        {
            var urlString = string.Empty;
            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
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
