using ImScoutAtWorker.Models;
using System.Globalization;
using HtmlAgilityPack;
public static class FlatParser
{
    public static async Task<Flat> ParseAsync(string url)
    {
        using var http = new HttpClient();
        var html = await http.GetStringAsync(url);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var flat = new Flat
        {
            Url = url,
            CreatedAt = DateTime.UtcNow
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
            if (decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out var p))
                flat.Price = p;
        }

        // Rooms & Area
        var info = doc.DocumentNode.SelectSingleNode("//*[contains(text(),'m²') and contains(text(),'Zimmer')]");
        if (info != null)
        {
            var parts = info.InnerText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (part.EndsWith("m²") && double.TryParse(part.Replace("m²", "").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var a))
                    flat.Area = a;
                if (part.EndsWith("Zimmer") && int.TryParse(part.Replace("Zimmer", "").Trim(), out var r))
                    flat.Rooms = r;
            }
        }

        // Description
        var descNode = doc.DocumentNode.SelectSingleNode("//h3[contains(text(),'Beschreibung')]/following-sibling::p");
        if (descNode != null)
            flat.Description = descNode.InnerText.Trim();

        // Address / Postal code / City
        var addrNode = doc.DocumentNode.SelectSingleNode("//*[contains(text(),'Adresse anfragen')]/preceding-sibling::div");
        if (addrNode != null)
        {
            var lines = addrNode.InnerText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length >= 2)
            {
                var pcodeCity = lines[^1].Trim();
                var parts2 = pcodeCity.Split(' ', 2);
                if (parts2.Length == 2)
                {
                    flat.PostalCode = parts2[0];
                    flat.City = parts2[1];
                }
                flat.Address = lines[0].Trim();
            }
        }

        return flat;
    }
}
