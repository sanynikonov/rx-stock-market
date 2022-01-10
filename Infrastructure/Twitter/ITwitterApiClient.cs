using Infrastructure.Users;
using Tweetinvi.Models.V2;

namespace Infrastructure.Twitter;

public interface ITwitterApiClient
{
    IObservable<TweetV2> GetTweets(CompanyModel companyModel);
}