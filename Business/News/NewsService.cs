using System.Reactive.Linq;
using Business.ObservableExtensions;
using Infrastructure.Twitter;
using Infrastructure.Users;

namespace Business.News;

public class NewsService : INewsService
{
    private readonly ITwitterApiClient _twitterApiClient;

    public NewsService(ITwitterApiClient twitterApiClient)
    {
        _twitterApiClient = twitterApiClient;
    }

    public IObservable<NewsModel> GetRecentCompanyNews(DateTimeOffset from, DateTimeOffset to, CompanyModel company)
    {
        return _twitterApiClient
            .GetTweets(company, from.UtcDateTime, to.UtcDateTime)
            .Select(tweet => new NewsModel
            {
                CreatedAt = tweet.CreatedAt,
                Content = tweet.Text
            }).RetryWithBackoffStrategy();
    }
}