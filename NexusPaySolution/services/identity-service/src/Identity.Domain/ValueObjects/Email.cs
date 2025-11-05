using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Identity.Domain.Exceptions;

namespace Identity.Domain.ValueObjects
{
    public class Email : ValueObject
    {
        private Email() { }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidEmailException("email connot be null or empty string");
            }

            if (!IsValidEmail(value))
            {
                throw new InvalidEmailException("invalid email format");
            }

            Value = value.Trim().ToLower();
        }

        public string Value { get; private set; }

        private bool IsValidEmail(string email)
        {
            try
            {
                var address = new MailAddress(email);

                return address.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public override string ToString() => Value;
    }
}
