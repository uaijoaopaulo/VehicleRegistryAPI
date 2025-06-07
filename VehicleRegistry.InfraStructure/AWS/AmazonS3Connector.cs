using Amazon.S3;
using Amazon.S3.Model;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws;

namespace VehicleRegistry.InfraStructure.AWS
{
    public class AmazonS3Connector(IAmazonS3 amazonS3Client) : IAmazonS3Connector
    {
        private readonly IAmazonS3 _amazonS3Client = amazonS3Client;

        public string GeneratePresignedUrl(string bucketName, string fileName, string contentType)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = fileName,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(15),
                ContentType = contentType
            };

            return _amazonS3Client.GetPreSignedURL(request);
        }
    }
}
