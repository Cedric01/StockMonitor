using System.Text.Json;

namespace StockMonitor.Services;

public class FinnhubProvider : IMarketDataProvider
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public FinnhubProvider(HttpClient httpClient, IConfiguration config)
    {
        _http = httpClient;
        _apiKey = config["Finnhub:ApiKey"];
    }

    public async Task<decimal?> GetPriceAsync(string symbol)
    {
        var url = $"https://finnhub.io/api/v1/quote?symbol={symbol}&token={_apiKey}";

        var json = await _http.GetStringAsync(url);

        using var doc = JsonDocument.Parse(json);

        // Finnhub returns:
        // { "c": 174.30, "h": 175.10, "l": 171.0, "o": 172.5, "pc": 173.0 }

        if (!doc.RootElement.TryGetProperty("c", out var currentPrice))
            return null;

        return currentPrice.GetDecimal();
    }
}
