using MediatR;
using System.ComponentModel.DataAnnotations;
using Transaction.Application.TransactionInfo.DTOs;

namespace Transaction.Application.TransactionInfo.Queries
{
    public class GetUserTransactionsQuery : IRequest<List<TransactionDto>>
    {
        [Required]
        public Guid UserId { get; set; }
    }
}
