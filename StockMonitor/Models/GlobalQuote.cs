using System.Globalization;
using System.Text.Json.Serialization;

namespace StockMonitor.Models;

public class GlobalQuote
{
    public decimal? Price { get; set; }

    [JsonPropertyName("05. price")]
    public string? PriceRaw
    {
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Price = null;
                return;
            }

            Price = decimal.TryParse(value, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var p)
                ? p
                : null;
        }
    }
}
