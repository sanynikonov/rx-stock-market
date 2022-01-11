namespace Business.Stock.Trend;

public interface IStockTrendSubscriptionService
{
    IObservable<TrendInfoModel> GetTrend(string username);
}