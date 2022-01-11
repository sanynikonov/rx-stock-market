using System.Reactive.Linq;
using Infrastructure.Users;

namespace Business.News;

public class NewsServiceStub : INewsService
{
    public IObservable<NewsModel> GetRecentCompanyNews(DateTimeOffset @from, DateTimeOffset to, CompanyModel company)
    {
        return Observable.Empty<NewsModel>();
    }
}