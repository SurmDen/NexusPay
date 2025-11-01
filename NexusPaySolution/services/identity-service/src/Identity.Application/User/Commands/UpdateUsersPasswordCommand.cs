using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Commands
{
    public class UpdateUsersPasswordCommand : IRequest
    {
        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }
    }
}
