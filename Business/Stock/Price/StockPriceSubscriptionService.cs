using System.Reactive.Linq;
using Grpc.Core;

namespace Business.Stock.Price;

public class StockPriceSubscriptionService : IStockPriceSubscriptionService
{
    private readonly IStockService _stockService;

    public StockPriceSubscriptionService(IStockService stockService)
    {
        _stockService = stockService;
    }

    public IObservable<StockTimeSeries> GetStockTimeSeries(StreamRequest request)
    {
        return request.TimeInterval == 1
            ? _stockService.GetCompanyPriceChangeByUserPreferences(request.Company)
                .Buffer(request.TimeInterval)
                .Select(buffer => buffer[request.TimeInterval - 1])
            : _stockService.GetCompanyPriceChangeByUserPreferences(request.Company);
    }
}

public class StreamRequest
{
    public string Company { get; set; }
    public int TimeInterval { get; set; }
}