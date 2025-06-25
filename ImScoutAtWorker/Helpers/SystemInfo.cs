public static class SystemInfo
{
    public static void InitValues(IConfiguration configuration)
    {
        RabbitMQHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST")
        ?? configuration.GetValue("RABBITMQ_HOST", string.Empty);

        RabbitMQPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT")
        ?? configuration.GetValue("RABBITMQ_PORT", string.Empty);

        RABBIT_USER = configuration.GetValue("RABBITMQ_USER", string.Empty);
        RABBIT_PASSWORD = configuration.GetValue("RABBITMQ_PASSWORD", string.Empty);

        SQL_CONNECTION_STRING = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
        ?? configuration.GetValue("SQL_CONNECTION_STRING", string.Empty);

        PARSER_URL = Environment.GetEnvironmentVariable("PARSER_URL")
        ?? configuration.GetValue("PARSER_URL", string.Empty);

    }
    public static string RabbitMQHost = string.Empty;
    public static string RabbitMQPort = string.Empty;
    public static DateTime StartTime = DateTime.Now;
    public static string RABBIT_USER = string.Empty;
    public static string RABBIT_PASSWORD = string.Empty;
    public static string SQL_CONNECTION_STRING = string.Empty;
    public static string PARSER_URL = string.Empty;
}