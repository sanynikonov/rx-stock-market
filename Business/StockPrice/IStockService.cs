namespace Business;

public interface IStockService
{
    IObservable<StockTimeSeries> GetCompanyPriceChangeByUserPreferences(string company);
}