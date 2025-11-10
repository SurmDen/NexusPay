
namespace Transaction.Application.Interfaces
{
    public interface IProducer
    {
        public Task SendMessage(string routingKey, string message);

        public Task SendObject<T>(string routingKey, T obj);
    }
}
