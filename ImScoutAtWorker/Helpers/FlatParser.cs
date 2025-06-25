using ImScoutAtWorker.Models;
using System.Globalization;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
public static class FlatParser
{
    public static async Task<Flat> ParseAsync(RabbitMQMessage model)
    {
        using var http = new HttpClient();
        var html = await http.GetStringAsync(model.Link);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var flat = new Flat
        {
            Url = model.Link,
            CreatedAt = DateTime.UtcNow,
            Hash = model.Hash
        };

        // ImScout ID from cwId field in JSON embedded
        var script = doc.DocumentNode.SelectSingleNode("//script[contains(text(),'obj_cwId')]");
        if (script != null)
        {
            var m = System.Text.RegularExpressions.Regex.Match(script.InnerText, @"obj_cwId"":""(?<cw>[^""]+)""");
            if (m.Success)
                flat.ImScoutId = m.Groups["cw"].Value;
        }

        // Title
        flat.Title = doc.DocumentNode.SelectSingleNode("//h1")?.InnerText.Trim();

        // Price
        var priceNode = doc.DocumentNode.SelectSingleNode("//*[contains(text(),'€') and contains(text(),'.')]");
        if (priceNode != null)
        {
            var priceText = priceNode.InnerText.Replace("€", "")
                                               .Replace(".", "")
                                               .Replace(",", ".")
                                               .Trim();
            if (decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
                flat.Price = price;
            else if (decimal.TryParse(model.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out var modelPrice))
                flat.Price = modelPrice;
        }

        // Rooms & Area
        var info = doc.DocumentNode.SelectSingleNode("//*[contains(text(),'m²') and contains(text(),'Zimmer')]");
        try
        {
            if (info != null)
            {
                var parts = info.InnerText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];
                    if (part == "m²")
                    {
                        if (double.TryParse(parts[i - 1], NumberStyles.Any, CultureInfo.InvariantCulture, out var area))
                        {
                            flat.Area = area;
                        }
                        else if (double.TryParse(model.Area.Replace("m²", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var modelArea))
                            flat.Area = modelArea;
                    }
                    else if (part == "Zimmer")
                    {
                        if (double.TryParse(parts[i - 1], NumberStyles.Any, CultureInfo.InvariantCulture, out var rooms))
                        {
                            flat.Rooms = rooms;
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            // Skip parsing Rooms and Area
        }


        // Description
        var descNode = doc.DocumentNode.SelectSingleNode("//h3[contains(text(),'Beschreibung')]/following-sibling::p");
        if (descNode != null)
            flat.Description = descNode.InnerText.Trim();

        // City
        var cityNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'expose__address')]");
        if (cityNode != null)
        {
            var addrText = cityNode.InnerText.Trim();
            var lines = addrText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            if (lines.Length >= 2)
            {
                var pcodeCity = lines[1];
                var parts2 = pcodeCity.Split(' ', 2);
                if (parts2.Length == 2)
                {
                    flat.City = parts2[1];
                }
                else
                {
                    flat.City = model.City;
                }
            }
            else
            {
                flat.City = model.City;
            }
        }
        if (string.IsNullOrWhiteSpace(flat.City))
        {
            flat.City = model.City;
        }

        return flat;
    }
}
