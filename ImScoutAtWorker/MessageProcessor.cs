using ImScoutAtWorker.Models;
using ImScoutAtWorker.Interfaces;
using System.Text.Json;
using RabbitMQ.Client.Events;

namespace ImScoutAtWorker
{
    public class MessageProcessor : IMessageProcessor
    {
        public async Task MessageReceivedHandler(RabbitMQMessage model)
        {
            var flat = await FlatParser.ParseAsync(model.Link);
        }
    }
}