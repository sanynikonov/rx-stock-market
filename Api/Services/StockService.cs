using System.Reactive.Linq;
using Api.Extensions;
using AutoMapper;
using Business.Stock;
using Business.Stock.Price;
using Grpc.Core;

namespace Api.Services;

public class StockService : StockMarketService.StockMarketServiceBase
{
    private readonly ILogger<StockService> _logger;
    private readonly IMapper _mapper;
    private readonly IStockPriceSubscriptionService _service;
    private readonly IStockService _stockService;

    public StockService(IStockPriceSubscriptionService service, IStockService stockService, IMapper mapper, ILogger<StockService> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
        _stockService = stockService;
    }

    public override async Task GetStockPriceStream(PriceRequest request, IServerStreamWriter<PriceStreamResponse> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("Opened stream");

        var tcs = new TaskCompletionSource();
        context.CancellationToken.Register(() => tcs.SetResult());

        using var _ = _service
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
}