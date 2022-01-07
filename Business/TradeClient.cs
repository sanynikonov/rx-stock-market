using System.Net.Http.Json;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.Json;

namespace Business;

public class TradeClient
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly HttpClient _httpClient;

    public TradeClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public IObservable<TradeModel?> GetTrades()
    {
        return Observable.Interval(TimeSpan.FromSeconds(10))
            .SelectMany(async _ =>
            {
                var bytes = await _httpClient.GetByteArrayAsync(
                    "https://query1.finance.yahoo.com/v8/finance/chart/AAPL?range=5m&interval=1m");
                return JsonSerializer.Deserialize<TradeModel>(bytes, Options);
            });
    }
}