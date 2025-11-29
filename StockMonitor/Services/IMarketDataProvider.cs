namespace StockMonitor.Services;

public interface IMarketDataProvider
{
    Task<decimal?> GetCurrentPriceAsync(string symbol);
}
