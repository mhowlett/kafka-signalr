using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalR
{
    public class MessagerHub : Hub<IMessagerHub>
    {
        private IProducerService _producer;
        public MessagerHub(IProducerService producerService)
        {
            _producer = producerService;
        }

        // https://docs.microsoft.com/en-us/aspnet/core/signalr/javascript-client?view=aspnetcore-2.2
        public async Task ProduceMessage(string message)
        {
            await _producer.Produce(new Message<Null, string> { Value = message });
        }

        public async Task SendMessageToClients(string message)
        {
            await Clients.All.SendMessage(message);
        }
    }
}