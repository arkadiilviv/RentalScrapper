public static class SystemInfo
{
    public static void InitValues(IConfiguration configuration)
    {
        DelaySeconds = configuration.GetValue("DELAY_SECONDS", 5);
        RetryCount = configuration.GetValue("RETRY_COUNT", 5);

        ScrapeUrl = Environment.GetEnvironmentVariable("SCRAPE_URL")
        ?? configuration.GetValue("SCRAPE_URL", string.Empty);

        RabbitMQHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST")
        ?? configuration.GetValue("RABBITMQ_HOST", string.Empty);

        RabbitMQPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT")
        ?? configuration.GetValue("RABBITMQ_PORT", string.Empty);

        RABBIT_USER = configuration.GetValue("RABBITMQ_USER", string.Empty);
        RABBIT_PASSWORD = configuration.GetValue("RABBITMQ_PASSWORD", string.Empty);

    }
    public static int DelaySeconds = 5;
    public static int RetryCount = 3;
    public static string ScrapeUrl = string.Empty;
    public static string RabbitMQHost = string.Empty;
    public static DateTime StartTime = DateTime.Now;
    public static string RABBIT_USER = string.Empty;
    public static string RABBIT_PASSWORD = string.Empty;
    public static string RabbitMQPort = string.Empty;
}