namespace StockMonitor.Services;

public interface IMarketDataProvider
{
    Task<decimal?> GetPriceAsync(string symbol);
}
