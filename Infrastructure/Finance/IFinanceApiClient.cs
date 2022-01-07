namespace Infrastructure.Finance;

public interface ITradeClient
{
    IObservable<FinanceChartModel> GetTrades();
}