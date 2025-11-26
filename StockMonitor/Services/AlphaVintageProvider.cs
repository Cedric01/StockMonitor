using StockMonitor.Models;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace StockMonitor.Services;

public class AlphaVantageProvider : IMarketDataProvider
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public AlphaVantageProvider(HttpClient http, IConfiguration config)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _apiKey = config?["AlphaVantage:ApiKey"] ?? throw new ArgumentException("Missing AlphaVantage:ApiKey", nameof(config));
    }

    public async Task<decimal?> GetCurrentPriceAsync(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol must be provided.", nameof(symbol));

        var url = $"query?function=GLOBAL_QUOTE&symbol={Uri.EscapeDataString(symbol)}&apikey={_apiKey}";

        AlphaVantageResponse? response;
        try
        {
            response = await _http.GetFromJsonAsync<AlphaVantageResponse>(url);
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (NotSupportedException)
        {
            return null;
        }
        catch (System.Text.Json.JsonException)
        {
            return null;
        }

        return response?.GlobalQuote?.Price;
    }
}
