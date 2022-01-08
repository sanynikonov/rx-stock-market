namespace Business.StockPrice;

public interface IStockService
{
    IObservable<StockTimeSeries> GetCompanyPriceChangeByUserPreferences(string company);
}