using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalR
{
    public interface IMessagerHub
    {
        Task ProduceMessage(string message);

        Task SendMessage(string message);
    }
}