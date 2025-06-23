using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using ImScoutAtWorker.Interfaces;

namespace ImScoutAtWorker
{
    public class RabbitMQClient
    {
        private IMessageProcessor _messageProcessor;
        public RabbitMQClient(IMessageProcessor messageProcessor)
        {
            _messageProcessor = messageProcessor;
        }
        public async Task ReceiveMessages()
        {
            var factory = new ConnectionFactory
            {
                HostName = SystemInfo.RabbitMQHost,
                Port = int.Parse(SystemInfo.RabbitMQPort),
                UserName = SystemInfo.RABBIT_USER,
                Password = SystemInfo.RABBIT_PASSWORD
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "imscoutat",
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += OnMessageReceivedAsync;

            await channel.BasicConsumeAsync("imscoutat", autoAck: true, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private async Task<bool> OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var deserializedMessage = JsonSerializer.Deserialize<Models.RabbitMQMessage>(message);
            await _messageProcessor.MessageReceivedHandler(deserializedMessage);
            Console.WriteLine($" [x] Received {message}");
            return true;
        }
    }
}