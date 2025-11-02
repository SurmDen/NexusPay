using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Identity.Application.User.Commands
{
    public class DeleteUserCommand : IRequest
    {
        [Required]
        public Guid UserId { get; set; }
    }
}
