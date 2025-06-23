using System.Text.Json;

namespace ImScoutAT;

public class RabbitMQMessage
{
    public string Hash { get; set; }
    public string Link { get; set; }
    public string Price { get; set; }
    public string Area { get; set; }
    public override string ToString() => JsonSerializer.Serialize(this);
}