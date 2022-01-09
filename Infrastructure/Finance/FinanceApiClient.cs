using System.Reactive.Linq;
using System.Text.Json;

namespace Infrastructure.Finance;

public class FinanceApiApiClient : IFinanceApiClient
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly HttpClient _httpClient;

    public FinanceApiApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public IObservable<FinanceChartModel> GetTrades(string company)
    {
        return Observable.Interval(TimeSpan.FromMinutes(1))
            .SelectMany(async _ =>
            {
                var bytes = await _httpClient.GetByteArrayAsync(
                    $"https://query1.finance.yahoo.com/v8/finance/chart/{company}?range=1m&interval=1m");
                return JsonSerializer.Deserialize<FinanceChartModel>(bytes, Options);
            })!;
    }
}