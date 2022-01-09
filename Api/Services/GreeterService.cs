using System.Reactive;
using System.Reactive.Linq;
using Api;
using Api.Extensions;
using Grpc.Core;

namespace Api.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override async Task SayHello(HelloRequest request, IServerStreamWriter<HelloReply> streamWriter, ServerCallContext context)
        {
            _logger.LogInformation("Opened stream");

            var tcs = new TaskCompletionSource();
            context.CancellationToken.Register(() => tcs.SetResult());

            var observable = GetObservable(); //should be service call

            using var _ = observable
                .SubscribeAsync(async data
                    => await streamWriter.WriteAsync(new HelloReply { Message = "Hello " + data }));

            await tcs.Task;

            _logger.LogInformation("Closed stream");
        }

        private static IObservable<string> GetObservable()
        {
            return Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Select(i => i.ToString());
        }
    }
}