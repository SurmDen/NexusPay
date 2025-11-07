using MediatR;

namespace Wallet.Application.Wallet.Queries
{
    public class GetBalanceQuery : IRequest<decimal>
    {
        public Guid UserId { get; set; }
    }
}
