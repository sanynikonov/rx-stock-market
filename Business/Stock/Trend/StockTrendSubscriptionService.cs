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
        (TrendInfoModel Previous, TrendInfoModel Current) trendTuple = (new TrendInfoModel(), new TrendInfoModel());

        return _userRepository.GetUserPreferences(username)
            .Do(u =>
            {
                if (u == null)
                    throw new InvalidOperationException("User not found.");
                if (!u.RequestedCompanies.Any())
                    throw new InvalidOperationException("User preferences not found.");
            })
            .SelectMany(u => u.RequestedCompanies)
            .SelectMany(company =>
                _stockService.GetCompanyPriceChangeByUserPreferences(company.Name)
                    .CalculatePriceChangesByNeighborsHigh()
                    .Where(p => p.PriceChange != 0)
                    .Scan(trendTuple, (pair, model) => 
                        (pair.Current, model))
                    .Where(pair =>
                        pair.Current.PriceChange * pair.Previous.PriceChange < 0)
                    .Select(pair => pair.Current)
                    .Scan(trendTuple, (pair, model) =>
                        (pair.Current, model))
                    .Select(pair => new TrendInfoModel
                    {
                        Company = pair.Current.Company,
                        Currency = pair.Current.Currency,
                        PriceChange = pair.Current.PriceChange,
                        Timestamp = pair.Current.Timestamp,
                        News = _newsService.GetRecentCompanyNews(
                            DateTimeOffset.FromUnixTimeSeconds(pair.Previous.Timestamp),
                            DateTimeOffset.FromUnixTimeSeconds(pair.Current.Timestamp), company).ToEnumerable()
                    }));
    }
}