using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Commands
{
    public class ConfirmEmailCommand : IRequest<bool>
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public int Code { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}
