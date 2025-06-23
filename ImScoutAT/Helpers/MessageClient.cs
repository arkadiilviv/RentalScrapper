using RabbitMQ.Client;
using System.Text;
namespace ImScoutAT;

public class MessageClient
{
    private readonly ILogger<MessageClient> _logger;
    private readonly string _connectionString;
    private readonly string _user;
    private readonly string _password;
    private readonly int MAX_RetryCount;
    private readonly int MAX_RetrySeconds;
    private int RetryCount = 0;

    public MessageClient(ILogger<MessageClient> logger)
    {
        _logger = logger;
        _connectionString = SystemInfo.RabbitMQHost;
        MAX_RetryCount = SystemInfo.RetryCount;
        MAX_RetrySeconds = SystemInfo.DelaySeconds;
        _user = SystemInfo.RABBIT_USER;
        _password = SystemInfo.RABBIT_PASSWORD;
    }

    public async Task<bool> Send(RabbitMQMessage itemModel, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return false;
        try
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = SystemInfo.RabbitMQHost,
                Port = Convert.ToInt32(SystemInfo.RabbitMQPort),
                UserName = _user,
                Password = _password
            };
            
            using var connection = await connectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.BasicPublishAsync(exchange: string.Empty,
                                            routingKey: "imscoutat",
                                            body: Encoding.UTF8.GetBytes(itemModel.ToString()));
        }
        catch (Exception ex)
        when (ex is RabbitMQ.Client.Exceptions.ConnectFailureException ||
              ex is RabbitMQ.Client.Exceptions.BrokerUnreachableException)
        {
            if (RetryCount++ < MAX_RetryCount)
            {
                if (stoppingToken.IsCancellationRequested)
                    return false;
                _logger.LogWarning("RabbitMQ connection failed for {0}, retrying in {1} seconds", itemModel.Hash, MAX_RetrySeconds);
                await Task.Delay(MAX_RetrySeconds * 1000, stoppingToken);
                await Send(itemModel, stoppingToken);
            }
            RetryCount = 0;
            return false;
        }

        return true;
    }
}