namespace ImportWizard.WebApi.Services.Interfaces
{
    public interface IMessagePublisher
    {
        /// <summary>
        /// Serialize and publish a message of type T to the configured topic.
        /// </summary>
        Task PublishAsync<T>(T message);
    }
}
