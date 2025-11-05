using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Infrastructure.Services
{
    public class SmtpSettings
    {
        public string Host { get; set; } = "smtp.gmail.com";

        public int Port { get; set; } = 587;

        public string Username { get; set; } = "surmanidzedenis609@gmail.com";

        public string Password { get; set; } = "knqs wvxl tzjc rgam";

        public bool EnableSsl { get; set; } = true;

        public string FromName { get; set; } = "NexusPay";

        public string FromEmail { get; set; } = "surmanidzedenis609@gmail.com";
    }
}
