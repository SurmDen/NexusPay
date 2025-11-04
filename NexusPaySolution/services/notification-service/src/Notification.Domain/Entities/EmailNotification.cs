using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Domain.Entities
{
    public class EmailNotification : Notification
    {
        private EmailNotification()
        {
            
        }

        public EmailNotification(string message, string email, string sub, DateTime? occuredOn, bool isSuccess)
        {
            Id = Guid.NewGuid();

            if (occuredOn == null)
            {
                throw new ArgumentNullException("notification time was null");
            }

            OccuredOn = (DateTime)occuredOn;

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("message cannot be null or empty");
            }

            Body = message;

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("email cannot be null or empty");
            }

            Email = email;

            if (string.IsNullOrWhiteSpace(sub))
            {
                throw new ArgumentException("sub cannot be null or empty");
            }

            Subject = sub;

            IsSuccess = isSuccess;
        }

        public string Email { get; private set; }

        public string Subject { get; private set; }
    }
}
