namespace Business;

public interface ITradeClient
{
    IObservable<TradeModel> GetTrades();
}