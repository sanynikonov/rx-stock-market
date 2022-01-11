using System.Reactive.Disposables;
using System.Reactive.Linq;
using Infrastructure.Finance;
using Infrastructure.Storage;

namespace Business.Stock;

public class StockServiceRandomized : IStockService
{
    private readonly IFinanceApiClient _financeApiClient;
    private readonly IStorage<string, (IObservable<StockTimeSeries> Stream, int SubscribersCount)> _streams;
    private readonly Random _random = new();

    public StockServiceRandomized(IFinanceApiClient financeApiClient, IStorage<string, (IObservable<StockTimeSeries> Stream, int SubscribersCount)> streamsStorage)
    {
        _financeApiClient = financeApiClient;
        _streams = streamsStorage;
    }

    public IObservable<StockTimeSeries> GetCompanyPriceChangeByUserPreferences(string company)
    {
        if (_streams.TryGetValue(company, out var pair))
        {
            pair.SubscribersCount++;
            return pair.Stream;
        }

        var stream = _financeApiClient.GetTrades(company)
            .SelectMany(data => Observable.Create<StockTimeSeries>(observer =>
            {
                if (data.Chart.Error != null)
                {
                    observer.OnError(new Exception(data.Chart.Error.Description));
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                var result = data.Chart.Result.First();
                var quote = result.Indicators.Quote.First();

                for (int i = 0; i < result.Timestamp.Length; i++)
                {
                    observer.OnNext(new StockTimeSeries
                    {
                        Company = result.Meta.Symbol,
                        Currency = result.Meta.Currency,
                        Timestamp = result.Timestamp[i],
                        Open = quote.Open[i] + _random.NextDouble(),
                        Close = quote.Open[i] + _random.NextDouble(),
                        Low = quote.Low[i],
                        High = quote.High[i],
                        Volume = quote.Volume[i]
                    });
                }

                observer.OnCompleted();
                return Disposable.Empty;
            }))
            .Publish()
            .RefCount();

        _streams.Add(company, (stream, 1));
        return stream;
    }

    public void Unsubscribe(string company)
    {
        if (!_streams.TryGetValue(company, out var pair))
        {
            return;
        }

        pair.SubscribersCount--;

        if (pair.SubscribersCount <= 0)
        {
            _streams.Remove(company);
        }
    }
}