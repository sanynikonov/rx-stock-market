﻿using System.Reactive.Linq;
using System.Text.Json;
using Api;
using Business;
using Grpc.Core;
using Grpc.Net.Client;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

await StockServiceTest();

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