using ImScoutAtWorker.Models;

namespace ImScoutAtWorker.Interfaces
{
    public interface IMessageProcessor
    {
        public Task MessageReceivedHandler(RabbitMQMessage model);
    }
}