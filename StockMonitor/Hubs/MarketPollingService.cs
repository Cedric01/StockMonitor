using Microsoft.AspNetCore.SignalR;
using StockMonitor.Services;

namespace StockMonitor.Hubs;

public class MarketPollingService : BackgroundService
{
    private readonly IMarketDataProvider _provider;
    private readonly IHubContext<MarketHub> _hub;

    private readonly string[] _symbols = { "AAPL", "MSFT", "GOOGL" };

    public MarketPollingService(
        IMarketDataProvider provider,
        IHubContext<MarketHub> hub)
    {
        _provider = provider;
        _hub = hub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var result = new Dictionary<string, decimal>();

            foreach (var symbol in _symbols)
            {
                var price = await _provider.GetPriceAsync(symbol);

                if (price.HasValue)
                    result[symbol] = price.Value;
            }

            // Push to SignalR clients
            await _hub.Clients.All.SendAsync("ReceivePrices", result);


            // Poll every 5 seconds
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}

