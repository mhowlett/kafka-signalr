using Confluent.Kafka;
using System.Threading.Tasks;

namespace SignalR
{
    // a very cut-down kafka producer interface. in a real world app, you might
    // provide a domain specific abstraction instead that hides away use of kafka
    // altogether.
    public interface IProducerService
    {
        Task<DeliveryResult<Null, string>> Produce(Message<Null, string> message);
    }
}
