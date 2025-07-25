namespace ImScoutAtWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly RabbitMQClient _rabbitMQClient;

    public Worker(ILogger<Worker> logger, RabbitMQClient rabbitMQClient)
    {
        _rabbitMQClient = rabbitMQClient;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await _rabbitMQClient.ReceiveMessages();
            await Task.Delay(1000, stoppingToken);
        }
    }
}
