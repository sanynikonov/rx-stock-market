using System.Reactive.Linq;

namespace Business.Stock.Trend;

public static class ObservableExtensions
{
    public static IObservable<TrendInfoModel> CalculatePriceChangesByOpenClose(this IObservable<StockTimeSeries> input) =>
        input.Where(s =>
                s.Close.HasValue && s.Open.HasValue)
                    .Select(s => new TrendInfoModel
                    {
                        Timestamp = s.Timestamp,
                        Company = s.Company,
                        Currency = s.Currency,
                        PriceChange = s.Close - s.Open ?? 0
                    });

    public static IObservable<TrendInfoModel> CalculatePriceChangesByNeighborsHigh(
        this IObservable<StockTimeSeries> input)
    {
        (StockTimeSeries Previous, StockTimeSeries Current) stockTuple = (new StockTimeSeries(), new StockTimeSeries());
        return input.Scan(stockTuple, (pair, series) => (pair.Current, series))
            .Select(pair => new TrendInfoModel
            {
                Timestamp = pair.Current.Timestamp,
                Company = pair.Current.Company,
                Currency = pair.Current.Currency,
                PriceChange = pair.Current.High - pair.Previous.High ?? 0
            });
    }

    public static IObservable<TrendInfoModel> CalculatePriceChangesByNeighborsOpen(this IObservable<StockTimeSeries> input)
    {
        (StockTimeSeries Previous, StockTimeSeries Current) stockTuple = (new StockTimeSeries(), new StockTimeSeries());
        return input.Scan(stockTuple, (pair, series) => (pair.Current, series))
            .Select(pair => new TrendInfoModel
            {
                Timestamp = pair.Current.Timestamp,
                Company = pair.Current.Company,
                Currency = pair.Current.Currency,
                PriceChange = pair.Current.Open - pair.Previous.Open ?? 0
            });
    }
}