namespace Infrastructure.Finance;

public interface IFinanceApiClient
{
    IObservable<FinanceChartModel> GetTrades();
}