using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Transaction.Application.TransactionInfo.Commands
{
    public class CreateTransactionCommand : IRequest<Guid>
    {
        [Required]
        public Guid SenderId { get; set; }

        [Required]
        public Guid ReceiverId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string? Message { get; set; }
    }
}
