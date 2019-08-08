using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace SignalR
{
    public class ProducerService : IProducerService, IDisposable
    {
        private IProducer<Null, string> _producer;
        private string _topic;

        public ProducerService(IConfiguration config)
        {
            var producerConfig = new ProducerConfig();
            config.GetSection("Producer").Bind(producerConfig);
            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            _topic = config.GetValue<string>("Topic");
        }

        public Task<DeliveryResult<Null, string>> Produce(Message<Null, string> message)
        {
            return _producer.ProduceAsync(_topic, message);
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}
