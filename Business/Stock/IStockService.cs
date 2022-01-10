namespace Business.Stock;

public interface IStockService
{
    IObservable<StockTimeSeries> GetCompanyPriceChangeByUserPreferences(string company);
    void Unsubscribe(string company);
}