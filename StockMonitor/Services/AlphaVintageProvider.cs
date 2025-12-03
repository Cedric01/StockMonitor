using StockMonitor.Services;
using System.Text.Json;

public class AlphaVantageProvider : IMarketDataProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public AlphaVantageProvider(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["AlphaVantage:ApiKey"];
    }

    public async Task<decimal?> GetPriceAsync(string symbol)
    {
        var url = $"query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={_apiKey}&outputsize=compact";

        var json = await _httpClient.GetStringAsync(url);

        Console.WriteLine($"RAW JSON ({symbol}): {json}");

        using var doc = JsonDocument.Parse(json);

        // The JSON structure is:
        // {
        //   "Time Series (Daily)": {
        //        "2024-01-03": {
        //            "1. open": "144.23",
        //            "2. high": "146.00",
        //            "3. low": "143.95",
        //            "4. close": "145.32",
        //            "5. volume": "31232100"
        //        },
        //        ...
        //   }
        // }

        if (!doc.RootElement.TryGetProperty("Time Series (Daily)", out var series))
            return null;

        // First entry = most recent trading day
        var latest = series.EnumerateObject().FirstOrDefault();

        if (latest.Value.TryGetProperty("4. close", out var closeNode))
        {
            if (decimal.TryParse(closeNode.GetString(), out var closePrice))
                return closePrice;
        }

        return null;
    }
}
