using Api.Extensions;
using AutoMapper;
using Business.Stock;
using Business.Stock.Price;
using Business.Stock.Trend;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Api.Services;

public class StockService : StockMarketService.StockMarketServiceBase
{
    private readonly ILogger<StockService> _logger;
    private readonly IMapper _mapper;
    private readonly IStockPriceSubscriptionService _stockPriceSubscriptionService;
    private readonly IStockService _stockService;
    private readonly IStockTrendSubscriptionService _stockTrendSubscriptionService;

    public StockService(IStockPriceSubscriptionService stockPriceSubscriptionService,
        IStockTrendSubscriptionService stockTrendSubscriptionService,
        IStockService stockService,
        IMapper mapper,
        ILogger<StockService> logger)
    {
        _stockPriceSubscriptionService = stockPriceSubscriptionService;
        _stockTrendSubscriptionService = stockTrendSubscriptionService;
        _mapper = mapper;
        _logger = logger;
        _stockService = stockService;
    }

    [Authorize]
    public override async Task GetStockPriceStream(PriceRequest request, IServerStreamWriter<PriceStreamResponse> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("Opened stream");

        var tcs = new TaskCompletionSource();
        context.CancellationToken.Register(() => tcs.SetResult());

        using var _ = _stockPriceSubscriptionService
            .GetStockTimeSeries(_mapper.Map<StreamRequest>(request))
            .SubscribeAsync(
                async data =>
                {
                    var response = new PriceStreamResponse { Data = _mapper.Map<StockTimeSeries>(data) };
                    await responseStream.WriteAsync(response);
                },
                error =>
                {
                    _logger.LogError(error.Message);
                    responseStream.WriteAsync(new PriceStreamResponse
                        { Error = new Error { Message = error.Message } });
                });

        await tcs.Task;

        _stockService.Unsubscribe(request.Company);

        _logger.LogInformation("Closed stream");
    }

    [Authorize]
    public override async Task GetStockTrendStream(TrendRequest request, IServerStreamWriter<TrendStreamResponse> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("Opened stream");

        var tcs = new TaskCompletionSource();
        context.CancellationToken.Register(() => tcs.SetResult());
        var companies = new List<string>();

        using var _ = _stockTrendSubscriptionService
            .GetTrend(context.GetAuthorizedUserName())
            .SubscribeAsync(
                async data =>
                {
                    companies.Add(data.Company);
                    var response = new TrendStreamResponse { Data = _mapper.Map<TrendSeries>(data)};
                    await responseStream.WriteAsync(response);
                },
                error =>
                {
                    _logger.LogError(error.Message);
                    responseStream.WriteAsync(new TrendStreamResponse
                        { Error = new Error { Message = error.Message } });
                    tcs.SetResult();
                });

        await tcs.Task;

        foreach (var company in companies)
            _stockService.Unsubscribe(company);

        _logger.LogInformation("Closed stream");
    }
}