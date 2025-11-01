using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(string name, string email, Guid id);
    }
}
