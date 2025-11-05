
using Identity.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.ValueObjects
{
    public class RoleName : ValueObject
    {
        private RoleName()
        {
            
        }

        public RoleName(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new InvalidRoleException("Invalid role name");
            }

            Value = roleName;
        }

        public string Value { get; private set; }

        public override string ToString() => Value;
    }
}
