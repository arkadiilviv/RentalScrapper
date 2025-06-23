using Microsoft.Extensions.Caching.Memory;

namespace ImScoutAT;

public class DisplayHelper
{
    private ILogger<DisplayHelper> Logger { get; set; }
    private IMemoryCache Cache {get;set;}

    public DisplayHelper(ILogger<DisplayHelper> logger, IMemoryCache cache)
    {
        Logger = logger;
        Cache = cache;
    }

    public void DisplayInfo()
    {
        var stats = Cache.GetCurrentStatistics();
        Logger.LogInformation("🌍 Url: {0}", SystemInfo.ScrapeUrl);
        Logger.LogInformation("🐰 RabbitMQ: {0}:{1}", SystemInfo.RabbitMQHost, SystemInfo.RabbitMQPort);
        Logger.LogInformation("⌛ Delay: {0} seconds", SystemInfo.DelaySeconds);
        Logger.LogInformation("📅 Started at: {0}", SystemInfo.StartTime);
        Logger.LogInformation("⌚ Uptime: {0}", (DateTime.Now - SystemInfo.StartTime).ToString("hh\\:mm\\:ss"));
        Logger.LogInformation("📦 Items in cache: {0}", stats?.CurrentEntryCount);
        Logger.LogInformation("🗃️ Items estimated size: {0}", (stats?.CurrentEstimatedSize * 128 / 1048576.0) + " MB");
    }
}