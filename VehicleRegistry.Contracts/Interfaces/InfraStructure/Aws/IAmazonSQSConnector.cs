using VehicleRegistry.Contracts.InfraStructure.AWS.SQS;

namespace VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws
{
    public interface IAmazonSQSConnector
    {
        /// <summary>
        /// Sends a message to the specified Amazon SQS queue.
        /// </summary>
        /// <typeparam name="T">The type of the object to be sent.</typeparam>
        /// <param name="data">The object to send in the message body.</param>
        /// <param name="queueUrl">The URL of the SQS queue.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SendMessageToQueueAsync<T>(T data, string queueUrl);

        /// <summary>
        /// Retrieves messages from the specified Amazon SQS queue.
        /// </summary>
        /// <typeparam name="T">The type to which the message body should be deserialized.</typeparam>
        /// <param name="queueUrl">The URL of the SQS queue.</param>
        /// <param name="visibilityTimeout">
        /// The duration (in seconds) that the received messages are hidden from subsequent retrieve requests after being retrieved.
        /// </param>
        /// <returns>A task that represents the asynchronous operation, containing a list of messages.</returns>
        Task<List<SQSMessage<T>>> ReceiveMessagesAsync<T>(string queueUrl, int visibilityTimeout);

        /// <summary>
        /// Deletes a batch of messages from the specified Amazon SQS queue.
        /// </summary>
        /// <typeparam name="T">The type of the message body.</typeparam>
        /// <param name="messages">The list of messages to delete.</param>
        /// <param name="queueUrl">The URL of the SQS queue.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteMessagesInBatchAsync<T>(List<SQSMessage<T>> messages, string queueUrl);

        /// <summary>
        /// Deletes a single message from the specified Amazon SQS queue.
        /// </summary>
        /// <typeparam name="T">The type of the message body.</typeparam>
        /// <param name="message">The message to delete.</param>
        /// <param name="queueUrl">The URL of the SQS queue.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteMessageAsync<T>(SQSMessage<T> message, string queueUrl);
    }
}
