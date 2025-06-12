using VehicleRegistry.Contracts.InfraStructure.AWS.AWSConfig;

namespace VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws
{
    public interface IAmazonConnector
    {
        /// <summary>
        /// Retrieves the AWS queue settings for a given queue Id.
        /// </summary>
        /// <param name="queueId">The Id of the queue.</param>
        /// <returns>The settings of the specified AWS queue, or null if not found.</returns>
        AwsQueueSettings? GetQueueById(string queueId);

        /// <summary>
        /// Ensures that the required S3 bucket exists. Creates it if it does not exist.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task EnsureBucketAsync();

        /// <summary>
        /// Ensures that all required AWS SQS queues exist. Creates them if they do not exist.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task EnsureQueuesAsync();
    }
}
