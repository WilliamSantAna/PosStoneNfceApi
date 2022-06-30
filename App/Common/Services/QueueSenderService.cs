using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace PosStoneNfce.Common.Services
{
    class QueueSender
    {
        // connection string to your Service Bus namespace
        string connectionString = "<NAMESPACE CONNECTION STRING>";

        // the client that owns the connection and can be used to create senders and receivers
        ServiceBusClient client;

        // the sender used to publish messages to the queue
        ServiceBusSender sender;

        public QueueSender(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public async Task Send(string queueName, string messageContent)
        {
            client = new ServiceBusClient(connectionString);
            sender = client.CreateSender(queueName);

            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            // try adding a message to the batch
            if (!messageBatch.TryAddMessage(new ServiceBusMessage(messageContent)))
            {
                // if it is too large for the batch
                throw new Exception($"The message is too large to fit in the batch.");
            }

            try
            {
                // Use the producer client to send the batch of messages to the Service Bus queue
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch message has been published to the queue.");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }

    }
}