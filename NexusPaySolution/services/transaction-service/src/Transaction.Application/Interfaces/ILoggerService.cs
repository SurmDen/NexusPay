

namespace Transaction.Application.Interfaces
{
    public interface ILoggerService
    {
        public Task LogInfo(string message, string action);

        public Task LogWarning(string message, string action);

        public Task LogError(string message, string action, string? exception);
    }
}
