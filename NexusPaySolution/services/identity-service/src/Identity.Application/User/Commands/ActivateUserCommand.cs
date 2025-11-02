using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Identity.Application.User.Commands
{
    public class ActivateUserCommand : IRequest
    {
        [Required]
        public Guid UserId { get; set; }
    }
}
