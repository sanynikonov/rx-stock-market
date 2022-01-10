using System.Reactive.Linq;
using Infrastructure.Users;
using Microsoft.Extensions.Options;
using Tweetinvi;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;

namespace Infrastructure.Twitter;

public class TwitterApiClient : ITwitterApiClient
{
    private readonly TwitterApiClientSettings _settings;
    private readonly TwitterClient _twitterClient;

    public TwitterApiClient(IOptions<TwitterApiClientSettings> settings)
    {
        _settings = settings.Value;
        
        var credentials = new ConsumerOnlyCredentials(_settings.Key, _settings.Secret)
        {
            BearerToken = _settings.Token
        };
        
        _twitterClient = new TwitterClient(credentials);
    }

    public IObservable<TweetV2> GetTweets(CompanyModel companyModel)
    {
        string request = BuildTweetsRequest(companyModel);
        
        return Observable.FromAsync(async () =>
        {
            var search = await _twitterClient.SearchV2.SearchTweetsAsync(request);
            return search.Tweets;
        }).SelectMany(x => x);
    }

    private string BuildTweetsRequest(CompanyModel companyModel)
    {
        var initRequest = string.Empty;

        if (!companyModel.Name.IsNullOrEmpty())
            initRequest += $"#{companyModel.Name} ";

        if (!companyModel.Tags.IsNullOrEmpty())
        {
            foreach (var tag in companyModel.Tags) 
                initRequest += $"#{tag} ";
        }
        
        return initRequest.Trim();
    }
}