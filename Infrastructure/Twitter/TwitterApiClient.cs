using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Core.Models;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using Tweetinvi.Parameters.V2;
using Tweetinvi.Streaming.V2;
using Tweetinvi.Streams.Model;

namespace Infrastructure.Twitter;

public class TwitterApiClient : ITwitterApiClient
{
    private const string Key = "bUZJDz82U2xTSqMKgdKK1AInk";
    private const string Secret = "X17XzTRsDMA3dQUMPM813SZTwUhwJhcAvXReyoR9Rz2ImS3J95";

    private const string Token =
        "AAAAAAAAAAAAAAAAAAAAAM2XXwEAAAAAtdcdm42KluIcJkHCXIIPhJoq4RU%3DgLXipp5h9hSu3mftr7v12id7rqkIW0mjb5tIZm4GP653pfxvJk";

    private readonly TwitterClient _twitterClient;

    public TwitterApiClient()
    {
        var credentials = new ConsumerOnlyCredentials(Key, Secret)
        {
            BearerToken = Token
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
    
    public async Task GetTweets(string tag)
    {
        var stream = _twitterClient.Streams.CreateFilteredStream();
        stream.StallWarnings = null;
        stream.AddTrack(" ");
        stream.EventReceived += (sender, args) => { Console.WriteLine(args.Json); };
        await stream.StartMatchingAllConditionsAsync();
    }
    
    

    public async Task<TweetV2[]> GetTweets2(string tag)
    {
        var search = await _twitterClient.SearchV2.SearchTweetsAsync(tag);
        return search.Tweets;
    }
}