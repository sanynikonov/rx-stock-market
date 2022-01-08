using System.Reactive.Disposables;
using System.Reactive.Linq;
using Infrastructure;
using Infrastructure.Finance;
using Infrastructure.Storage;

namespace Business;

public class StockService : IStockService
{
    private readonly IFinanceApiClient _financeApiClient;
    private readonly IStorage<string, IObservable<StockTimeSeries>> _streams;

    public StockService(IFinanceApiClient financeApiClient, IStorage<string, IObservable<StockTimeSeries>> streamsStorage)
    {
        _financeApiClient = financeApiClient;
        _streams = streamsStorage;
    }

    public IObservable<StockTimeSeries> GetCompanyPriceChangeByUserPreferences(string company)
    {
        if (_streams.TryGetValue(company, out var stream))
        {
            return stream;
        }

        stream = _financeApiClient.GetTrades()
            .SelectMany(data => Observable.Create<StockTimeSeries>(observer =>
            {
                var result = data.Chart.Result.First();
                var quote = result.Indicators.Quote.First();

                for (int i = 0; i < result.Timestamp.Length; i++)
                {
                    observer.OnNext(new StockTimeSeries
                    {
                        Company = result.Meta.Symbol,
                        Currency = result.Meta.Currency,
                        Timestamp = result.Timestamp[i],
                        Open = quote.Open[i],
                        Close = quote.Close[i],
                        Low = quote.Low[i],
                        High = quote.High[i],
                        Volume = quote.Volume[i]
                    });
                }

                observer.OnCompleted();

                return Disposable.Empty;
            }));

        _streams.Add(company, stream);
        return stream;
    }
}