namespace Transaction.Domain.Models
{
    public class LogMessage
    {
        public string ServiceName { get; set; } = string.Empty;

        public string LogLevel { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string? Exception { get; set; }

        public string Action { get; set; } = string.Empty;
    }
}
