using ImScoutAtWorker.Models;
using ImScoutAtWorker.Interfaces;
using System.Text.Json;
using RabbitMQ.Client.Events;

namespace ImScoutAtWorker
{
    public class MessageProcessor : IMessageProcessor
    {
        public async Task MessageReceivedHandler(object model, BasicDeliverEventArgs ea)
        {
            // var rabbitMQMessage = JsonSerializer.Deserialize<RabbitMQMessage>(message);
            // if (rabbitMQMessage != null)
            // {
            //     // Process the message
            // }
        }
    }
}