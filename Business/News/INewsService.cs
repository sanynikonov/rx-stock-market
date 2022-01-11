namespace Business.News;

public interface INewsService
{
    IObservable<NewsModel> GetRecentNewsByUserPreferences(DateTimeOffset from, DateTimeOffset to, string username);
}