using System.Reactive.Linq;
using System.Text.Json;
using Business;

EventHandler MyEventHandler2 = null;

/*var httpClient = new HttpClient();
var tradeClient = new TradeClient(httpClient);

using var _ = tradeClient.GetTrades().Subscribe(trade =>
{
    Console.WriteLine(JsonSerializer.Serialize(trade));
}, e => Console.WriteLine(e.Message));

Task.Delay(TimeSpan.FromMinutes(2)).Wait();*/

Observable.Interval(TimeSpan.FromSeconds(1))
    .Subscribe(time => MyEventHandler2?.Invoke(null, EventArgs.Empty));

Observable.FromEventPattern(h => MyEventHandler2 += h, h => MyEventHandler2 -= h)
    .Subscribe(pattern => Console.WriteLine());

Task.Delay(50000).Wait();