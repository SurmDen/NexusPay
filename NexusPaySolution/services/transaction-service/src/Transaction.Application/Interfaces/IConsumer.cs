namespace Transaction.Application.Interfaces
{
    public interface IConsumer
    {
        public Task Subscribe<T>(string routingKey, string queueName);
    }
}
