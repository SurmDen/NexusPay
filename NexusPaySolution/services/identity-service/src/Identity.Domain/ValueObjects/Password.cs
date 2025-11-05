using Identity.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Identity.Domain.ValueObjects
{
    public class Password : ValueObject
    {
        private Password()
        {
            
        }

        public Password(string password)
        {
            Hash = GenerateHash(password);
        }

        public string Hash { get; private set; }

        private string GenerateHash(string value)
        {
            var sha256 = SHA256.Create();

            if (string.IsNullOrWhiteSpace(value) || value.Length < 6)
            {
                throw new InvalidPasswordException("password should be more than 6 characters");
            }

            var byteArray = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(byteArray);
            return Convert.ToBase64String(hash);
        }

        private bool ValidatePassword(string password)
        {
            return GenerateHash(password) == Hash;
        }

        public override string ToString() => Hash;
    }
}
