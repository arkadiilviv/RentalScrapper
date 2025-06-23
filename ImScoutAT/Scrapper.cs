using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using HtmlAgilityPack;
using System.Text;
using System.Security.Cryptography;

namespace ImScoutAT
{
    public interface IScrapper { }
    public class Scrapper : IScrapper
    {
        string page = "/seite-{0}";
        int pageCount = 1;
        private bool isStarted = false;
        private readonly MessageClient _messageClient;
        private readonly ILogger<Scrapper> _logger;
        private readonly IMemoryCache _cache;
        private DisplayHelper _displayHelper;
        private MemoryCacheEntryOptions options = new MemoryCacheEntryOptions() { Size = 1 };
        public Scrapper(ILogger<Scrapper> logger, IMemoryCache cache, MessageClient messageClient, DisplayHelper displayHelper)
        {
            _logger = logger;
            _cache = cache;
            _messageClient = messageClient;
            _displayHelper = displayHelper;
        }

        public async Task Scrape(string scrapeUrl, CancellationToken stoppingToken)
        {
            if (isStarted == false)
            {
                await InitialScrape(scrapeUrl, stoppingToken);
                isStarted = true;
            }
            else
            {
                Console.WriteLine("Scraping actual page");
                await ScrapeList(scrapeUrl, stoppingToken);
            }
        }

        public async Task InitialScrape(string scrapeUrl, CancellationToken stoppingToken)
        {
            bool scrapeRes = true;
            while (scrapeRes == true)
            {
                _displayHelper.DisplayInfo();
                Console.WriteLine("Scraping page {0}", pageCount);
                string pageUrl = String.Format(page, pageCount);
                string targetUrl = string.Empty;
                if (pageCount > 1)
                {
                    targetUrl = scrapeUrl + pageUrl;
                }
                else
                {
                    targetUrl = scrapeUrl;
                }

                scrapeRes = await ScrapeList(targetUrl, stoppingToken);
                pageCount++;
            }
        }

        private async Task<bool> ScrapeList(string scrapeUrl, CancellationToken stoppingToken)
        {
            var htmlWeb = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();
            bool loaded = false;

            while (!loaded)
            {
                try
                {
                    doc = await htmlWeb.LoadFromWebAsync(scrapeUrl);
                    loaded = true;
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("Can't connect {0}, retrying in {1} seconds...", nameof(scrapeUrl), SystemInfo.DelaySeconds);
                    await Task.Delay(SystemInfo.DelaySeconds * 1000);
                }
            }

            var nodes = doc.DocumentNode.SelectNodes("//li[@class=\"Item-item-J04\"]");
            if (nodes == null || nodes.Count == 0)
            {
                return false;
            }

            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = int.MaxValue,
                CancellationToken = stoppingToken
            };

            await Parallel.ForEachAsync(nodes, parallelOptions, async (node, token) =>
            {
                var item = ExtractItemModel(node);
                if (_cache.TryGetValue(item.Hash, out string? _))
                {
                    return;
                }

                if (await _messageClient.Send(item, token))
                {
                    _cache.Set(item.Hash, "cached", options);
                    Console.WriteLine("Item {0} added to cache", item.Link);
                }
            });

            return true;
        }

        private RabbitMQMessage ExtractItemModel(HtmlNode node)
        {
            var item = new RabbitMQMessage();

            var linkNode = node.SelectSingleNode(".//a[@class=\"Item-item__link-pTS\"]");
            item.Link = linkNode?.GetAttributeValue("href", string.Empty) ?? string.Empty;
            item.Hash = ComputeHash(item.Link);

            var priceNode = node.SelectSingleNode(".//li[@class=\"Text-color--gray-dark-sLZ Text-size--s-wTL Text-font-weight--bold-ax5 \"]");
            item.Price = priceNode?.InnerText ?? string.Empty;

            var areaNode = node.SelectSingleNode(".//li[@class=\"Text-color--gray-NvY Text-size--s-wTL \"]");
            item.Area = areaNode?.InnerText ?? string.Empty;

            return item;
        }

        private string ComputeHash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }
        }
    }
}