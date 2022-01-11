using Infrastructure.Users;

namespace Business.News;

public interface INewsService
{
    IObservable<NewsModel> GetRecentCompanyNews(DateTimeOffset from, DateTimeOffset to, CompanyModel company);
}