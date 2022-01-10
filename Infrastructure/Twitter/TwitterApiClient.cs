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

    /*public IObservable<TweetV2> GetTweets(string tag)
    {
        return Observable.Create<TweetV2>(async observer =>
        {
            var search = await _twitterClient.SearchV2.SearchTweetsAsync(tag);
            var tweets = search.Tweets;

            foreach (var tweet in tweets) 
                observer.OnNext(tweet);

            observer.OnCompleted();
            
            return Disposable.Empty;
        });
    }*/
    
    /*public IObservable<TweetV2> GetTweets(string tag)
    {
        return Observable
            .Interval(TimeSpan.FromSeconds(1))
            .SelectMany(async _ =>
            {
                var search = await _twitterClient.SearchV2.SearchTweetsAsync(tag);
                return search.Tweets;
            });
    }*/
    
    /*public IObservable<TweetV2> GetTweets(string tag)
    {
        return Observable
            .Interval(TimeSpan.FromSeconds(10))
            .Select(async _ => Observable.Create<TweetV2>(async observer =>
            {
                var search = await _twitterClient.SearchV2.SearchTweetsAsync(tag);
                var tweets = search.Tweets;

                foreach (var tweet in tweets) 
                    observer.OnNext(tweet);

                observer.OnCompleted();
            
                return Disposable.Empty;
            }));
    }*/
    
    /*public IObservable<TweetV2> GetTweets(string tag)
    {
        return Observable.Interval(TimeSpan.FromSeconds(10))
            .SelectMany()
    }*/
    
    /*public async Task GetTweets(string tag)
    {
        var stream = _twitterClient.Streams.CreateFilteredStream();
        stream.StallWarnings = null;
        stream.AddTrack(" ");
        stream.EventReceived += (sender, args) => { Console.WriteLine(args.Json); };
        await stream.StartMatchingAllConditionsAsync();
    }*/
    
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