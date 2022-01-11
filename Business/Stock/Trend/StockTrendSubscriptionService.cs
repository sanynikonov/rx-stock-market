using System.Reactive.Linq;
using Business.News;
using Infrastructure.Users;

namespace Business.Stock.Trend;

public class StockTrendSubscriptionService : IStockTrendSubscriptionService
{
    private readonly IStockService _stockService;
    private readonly IUserRepository _userRepository;
    private readonly INewsService _newsService;

    public StockTrendSubscriptionService(IStockService stockService, IUserRepository userRepository, INewsService newsService)
    {
        _stockService = stockService;
        _userRepository = userRepository;
        _newsService = newsService;
    }

    public IObservable<TrendInfoModel> GetTrend(string username)
    {
        (StockTimeSeries Previous, StockTimeSeries Current) stockTuple = (new StockTimeSeries(), new StockTimeSeries());
        (TrendInfoModel Previous, TrendInfoModel Current) trendTuple = (new TrendInfoModel(), new TrendInfoModel());

        return _userRepository.GetUserPreferences(username).SelectMany(u => u.RequestedCompanies.Select(x => x.Name))
            .SelectMany(company =>
                _stockService.GetCompanyPriceChangeByUserPreferences(company)
                    .Scan(stockTuple, (pair, series) => (pair.Current, series))
                    .Select(pair => new TrendInfoModel
                    {
                        Company = pair.Current.Company,
                        Currency = pair.Current.Currency,
                        PriceChange = pair.Current.High - pair.Current.High ?? 0
                    })
                    .Scan(trendTuple, (pair, model) => (pair.Current, model))
                    .Where(pair => pair.Current.PriceChange * pair.Previous.PriceChange > 0)
                    .Select(pair => pair.Current)
                    .Scan(trendTuple, (pair, model) => (pair.Current, model))
                    .Select(pair => new TrendInfoModel
                    {
                        Company = pair.Current.Company,
                        Currency = pair.Current.Currency,
                        PriceChange = pair.Current.PriceChange,
                        Timestamp = pair.Current.Timestamp,
                        News = _newsService.GetRecentNewsByUserPreferences(
                            DateTimeOffset.FromUnixTimeSeconds(pair.Previous.Timestamp),
                            DateTimeOffset.FromUnixTimeSeconds(pair.Current.Timestamp), username).ToEnumerable()
                    }));
    }
}