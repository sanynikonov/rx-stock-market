using System.Reactive.Linq;
using System.Reactive.Subjects;
using Api;
using Grpc.Core;
using Grpc.Net.Client;
using Infrastructure;
using Infrastructure.Storage;
using Infrastructure.Twitter;
using Infrastructure.Users;
using Microsoft.EntityFrameworkCore;

var token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJ1c2VyMTIzIiwibmJmIjoxNjQxODU5MjI1LCJleHAiOjE2NDI0NjQwMjUsImlhdCI6MTY0MTg1OTIyNX0.-Jr7Sw8Aqb2ydbXvj4ZlDjZdY8kjyiW5h3wWRGSNByiZTTegfGTE1uubBKFpdlcfrUQR0QNTRef_UB3WhWu0-Q";
var metadata = new Metadata {{"Authorization", $"Bearer {token}"}};
//await GetTweetsTest();
//await UpdatePreferencesUserServiceTest();
//await StockServiceTest();
await TrendStockServiceTest();
//await RepositoryTest();

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

    var response = await client.UpdatePreferencesAsync(new UpdatePreferencesRequest() { Companies = { new CompanyInfo { Name = "AAPL", SearchTags = { "finance", "stock" }} }}, metadata);

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

    var call = client.GetStockPriceStream(new PriceRequest { TimeInterval = 1, Company = "AAPL" }, metadata);

    await foreach (var data in call.ResponseStream.ReadAllAsync())
    {
        if (data.Error != null)
        {
            Console.WriteLine(data.Error.Message);
            continue;
        }
        Console.WriteLine(DateTime.Now.TimeOfDay + ": " + data.Data.Timestamp + ": Open - " + data.Data.Open + "; Close - " + data.Data.Close);
    }
}

async Task TrendStockServiceTest()
{
    var client = new StockMarketService.StockMarketServiceClient(GetGrpcChannel());

    var call = client.GetStockTrendStream(new TrendRequest(), metadata);
    
    await foreach (var data in call.ResponseStream.ReadAllAsync())
    {
        if (data.Error != null)
        {
            Console.WriteLine(data.Error.Message);
            continue;
        }

        Console.WriteLine(data.Data.Company + ": " + data.Data.PriceChange + " " + data.Data.Currency);
    }
}

async Task RepositoryTest()
{
    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=StockMarket;Integrated Security=True;");
    using var context = new AppDbContext(optionsBuilder.Options);
    var repository = new UserRepository(context, new KeyValueStorage<string, BehaviorSubject<UserModel>>());

    var obs = repository.GetUserPreferences("user123");
    obs.Subscribe(u => Console.WriteLine(u.NormalizedUserName));
    await Task.Delay(5000);
    obs.Subscribe(u => Console.WriteLine(u.NormalizedUserName));
    await Task.Delay(5000);
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