namespace ImScoutAtWorker.Interfaces
{
    public interface IMessageProcessor
    {
        public Task MessageReceivedHandler(object model, RabbitMQ.Client.Events.BasicDeliverEventArgs ea);
    }
}