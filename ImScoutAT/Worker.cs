using Microsoft.Extensions.Caching.Memory;
namespace ImScoutAT;

public class Worker : BackgroundService
{
    public static bool IsFirstRun = false;
    private readonly ILogger<Worker> _logger;
    private readonly Scrapper _scrapper;
    private readonly DisplayHelper _displayHelper;
    private readonly int DelaySeconds;
    private readonly string ScrapeUrl;

    public Worker(ILogger<Worker> logger, Scrapper scrapper, DisplayHelper displayHelper)
    {
        _logger = logger;
        _scrapper = scrapper;
        ScrapeUrl = SystemInfo.ScrapeUrl;
        DelaySeconds = SystemInfo.DelaySeconds;
        _displayHelper = displayHelper;
        if (String.IsNullOrEmpty(ScrapeUrl))
            throw new ArgumentNullException(nameof(ScrapeUrl));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _displayHelper.DisplayInfo();
            await _scrapper.Scrape(ScrapeUrl, stoppingToken);
            await Task.Delay(DelaySeconds * 1000, stoppingToken);
        }
    }
}