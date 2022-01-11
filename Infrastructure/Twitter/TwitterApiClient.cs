using System.Reactive.Linq;
using Infrastructure.Users;
using Microsoft.Extensions.Options;
using Tweetinvi;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using Tweetinvi.Parameters.V2;

namespace Infrastructure.Twitter;

public class TwitterApiClient : ITwitterApiClient
{
    private readonly TwitterApiClientSettings _settings;
    private readonly TwitterClient _twitterClient;

    //Test values
    public string Key = "bUZJDz82U2xTSqMKgdKK1AInk";
    public string Secret = "X17XzTRsDMA3dQUMPM813SZTwUhwJhcAvXReyoR9Rz2ImS3J95";

    public string Token =
        "AAAAAAAAAAAAAAAAAAAAAM2XXwEAAAAAtdcdm42KluIcJkHCXIIPhJoq4RU%3DgLXipp5h9hSu3mftr7v12id7rqkIW0mjb5tIZm4GP653pfxvJk";
    
    public TwitterApiClient(IOptions<TwitterApiClientSettings> settings)
    {
        _settings = settings.Value;
        
        var credentials = new ConsumerOnlyCredentials(_settings.Key, _settings.Secret)
        {
            BearerToken = _settings.Token
        };
        
        _twitterClient = new TwitterClient(credentials);
    }
    
    public TwitterApiClient()
    {
        var credentials = new ConsumerOnlyCredentials(Key, Secret)
        {
            BearerToken = Token
        };
        
        _twitterClient = new TwitterClient(credentials);
    }

    public IObservable<TweetV2> GetTweets(CompanyModel companyModel)
    {
        string request = BuildTweetsRequest(companyModel);
        
        var parameters = new SearchTweetsV2Parameters(request)
        {
            StartTime = DateTime.UtcNow.AddMinutes(-5)
        };
        
        return Observable.FromAsync(async () =>
        {
            var search = await _twitterClient.SearchV2.SearchTweetsAsync(parameters);
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