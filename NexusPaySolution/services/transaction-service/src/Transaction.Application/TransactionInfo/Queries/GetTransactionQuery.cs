using MediatR;
using System.ComponentModel.DataAnnotations;
using Transaction.Application.TransactionInfo.DTOs;

namespace Transaction.Application.TransactionInfo.Queries
{
    public class GetTransactionQuery : IRequest<TransactionDto>
    {
        [Required]
        public Guid TransactionId { get; set; }
    }
}
