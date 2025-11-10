using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction.Application.TransactionInfo.DTOs;

namespace Transaction.Application.TransactionInfo.Queries
{
    public class GetAllTransactiosQuery : IRequest<List<TransactionDto>>
    {
    }
}
