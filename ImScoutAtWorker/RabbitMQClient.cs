using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ImScoutAtWorker
{
    public class RabbitMQClient
    {
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

            static Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received {message}");
                return Task.CompletedTask;
            }

            await channel.BasicConsumeAsync("imscoutat", autoAck: true, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}