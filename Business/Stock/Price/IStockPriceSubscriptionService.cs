namespace Business.Stock.Price;

public interface IStockPriceSubscriptionService
{
    IObservable<StockTimeSeries> GetStockTimeSeries(StreamRequest request);
}