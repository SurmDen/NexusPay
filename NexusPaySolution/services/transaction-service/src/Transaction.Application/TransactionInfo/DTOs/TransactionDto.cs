using System.Security.Principal;

namespace Transaction.Application.TransactionInfo.DTOs
{
    public class TransactionDto
    {
        public DateTime Time { get; set; }

        public decimal Amount { get; set; }

        public string? Message { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; } = string.Empty;
    }
}
