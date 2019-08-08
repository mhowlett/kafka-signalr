using Confluent.Kafka;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace SignalR
{
    public class ConsumerService : IHostedService, IDisposable
    {
        // The Kafka Consumer doesn't provide async methods - don't try to shoehorn it into that
        // way of thinking, just make a long running background thread with a consume loop.
        private Thread _pollLoopThread;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private ConsumerConfig _consumerConfig = new ConsumerConfig();
        private string _topic;
        private IHubContext<MessagerHub, IMessagerHub> _messagerHubContext;

        // for more information on using SignalR with background services:
        // https://docs.microsoft.com/en-us/aspnet/core/signalr/background-services?view=aspnetcore-2.2
        public ConsumerService(IConfiguration config, IHubContext<MessagerHub, IMessagerHub> messagerHubContext)
        {
            config.GetSection("Consumer").Bind(_consumerConfig);
            _topic = config.GetValue<string>("Topic");
            _messagerHubContext = messagerHubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _pollLoopThread = new Thread(() => {
                try
                {
                    using (var consumer = new ConsumerBuilder<Null, string>(_consumerConfig).Build())
                    {
                        consumer.Subscribe(_topic);

                        try
                        {
                            while (!_cancellationTokenSource.IsCancellationRequested)
                            {
                                var cr = consumer.Consume(_cancellationTokenSource.Token);

                                _messagerHubContext.Clients.All.SendMessage($"received: {cr.Value}");
                            }
                        }
                        catch (OperationCanceledException) {}

                        consumer.Close();
                    }
                }
                catch
                {
                    // something bad happened. logic should be improved to ensure consumer is always
                    // operational over lifetime of background service.
                }
            });

            _pollLoopThread.Start();

            return Task.FromResult<object>(null);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                _cancellationTokenSource.Cancel();
                _pollLoopThread.Join();
            });
        }

        public void Dispose() {}
    }
}
