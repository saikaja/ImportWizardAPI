using System.Text.Json;
using Azure.Messaging.ServiceBus;
using ImportWizard.WebApi.Services.Interfaces;

namespace ImportWizard.WebApi.Services.Implementations
{
    public class ServiceBusMessagePublisher : IMessagePublisher
    {
        private readonly ServiceBusSender _sender;

        public ServiceBusMessagePublisher(ServiceBusSender sender)
        {
            _sender = sender;
        }

        public async Task PublishAsync<T>(T message)
        {
            var json = JsonSerializer.Serialize(message);
            var sbMsg = new ServiceBusMessage(json)
            {
                ContentType = "application/json"
            };
            await _sender.SendMessageAsync(sbMsg);
        }
    }
}
