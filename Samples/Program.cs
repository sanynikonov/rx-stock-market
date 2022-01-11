using System.Reactive.Linq;
using System.Text.Json;
using Api;
using Api.Services;
using Business;
using Grpc.Core;
using Grpc.Net.Client;
using Infrastructure;
using Infrastructure.Twitter;
using Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
using UserService = Api.UserService;

await GetTweetsTest();
await UpdatePreferencesUserServiceTest();

async Task GetTweetsTest()
{
    var company = new CompanyModel
    {
        Name = "meme"
    };

    var twitterClient = new TwitterApiClient();

    twitterClient.GetTweets(company).Subscribe(value =>
    {
        Console.WriteLine("-----------------------------------------------");
        Console.WriteLine(value.CreatedAt);
    });

    await Task.Delay(10000);
}

async Task UpdatePreferencesUserServiceTest()
{
    var client = new UserService.UserServiceClient(GetGrpcChannel());
    var token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJ1c2VyMTIzIiwibmJmIjoxNjQxODU5MjI1LCJleHAiOjE2NDI0NjQwMjUsImlhdCI6MTY0MTg1OTIyNX0.-Jr7Sw8Aqb2ydbXvj4ZlDjZdY8kjyiW5h3wWRGSNByiZTTegfGTE1uubBKFpdlcfrUQR0QNTRef_UB3WhWu0-Q";

    var response = await client.UpdatePreferencesAsync(new UpdatePreferencesRequest { Companies = { new CompanyInfo { Name = "AAPL", SearchTags = { "finance", "stock" }} }}, new Metadata { { "Authorization", $"Bearer {token}" } });

    if (response.Error != null)
    {
        Console.WriteLine(response.Error.Message);
    }
}

async Task LoginUserServiceTest()
{
    var client = new UserService.UserServiceClient(GetGrpcChannel());

    var response = await client.LoginAsync(new LoginRequest { UserName = "user123", Password = "!Password1" });

    Console.WriteLine(response.Error != null ? response.Error.Message : response.Token);
}

async Task RegisterUserServiceTest()
{
    var client = new UserService.UserServiceClient(GetGrpcChannel());

    var response = await client.RegisterAsync(new RegisterRequest { UserName = "user123", Password = "!Password1" });

    if (response.Error != null)
    {
        Console.WriteLine(response.Error.Message);
    }
}

async Task StockServiceTest()
{
    var client = new StockMarketService.StockMarketServiceClient(GetGrpcChannel());

    var call = client.GetStockPriceStream(new PriceRequest { TimeInterval = 1, Company = "AAPL" });
    await foreach (var data in call.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine(data.Data.Timestamp + ": " + data.Data.Open);
    }
}

async Task RxGrpcTest()
{
    var client = new Greeter.GreeterClient(GetGrpcChannel());

    var call = client.SayHello(new HelloRequest());
    await foreach (var data in call.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine(data.Message);
    }
}

GrpcChannel GetGrpcChannel() => GrpcChannel.ForAddress("https://localhost:7116", new GrpcChannelOptions
{
    HttpHandler = GetHttpClientHandler()
});

HttpClientHandler GetHttpClientHandler()
{
    var httpHandler = new HttpClientHandler();
    // Return `true` to allow certificates that are untrusted/invalid
    httpHandler.ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    return httpHandler;
}

void TestRx()
{
    /*var httpClient = new HttpClient();
    var tradeClient = new TradeClient(httpClient);

    using var _ = tradeClient.GetTrades().Subscribe(trade =>
    {
        Console.WriteLine(JsonSerializer.Serialize(trade));
    }, e => Console.WriteLine(e.Message));

    Task.Delay(TimeSpan.FromMinutes(2)).Wait();*/

    EventHandler MyEventHandler2 = null;

    Observable.Interval(TimeSpan.FromSeconds(1))
        .Subscribe(time => MyEventHandler2?.Invoke(null, EventArgs.Empty));

    Observable.FromEventPattern(h => MyEventHandler2 += h, h => MyEventHandler2 -= h)
        .Subscribe(pattern => Console.WriteLine());

    Observable.Range(0, 1)
        .Take(1)
        .Subscribe();

    Task.Delay(50000).Wait();
}