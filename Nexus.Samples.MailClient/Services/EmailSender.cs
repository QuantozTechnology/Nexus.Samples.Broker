using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Nexus.Samples.MailClient
{
    /// <summary>
    /// This class is used by the application to send email for account confirmation and password reset.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private static SmtpClient _client = null;
        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public Task SendEmailAsync(string email, List<string> CC, List<string> BCC, string subject, string message)
        {
            if (_client == null)
            {
                _client = new SmtpClient()
                {
                    Host = Options.Host,
                    Port = int.Parse(Options.Port),
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(Options.UserName, Options.Password)
                };
            }

            var mailMessage = new MailMessage()
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(email));

            if (CC != null)
            {
                foreach (var cc in CC)
                {
                    mailMessage.CC.Add(cc);
                }
            }

            if (BCC != null)
            {
                foreach (var bcc in BCC)
                {
                    mailMessage.Bcc.Add(bcc);
                }
            }

            var sender = _client.Credentials.GetCredential(_client.Host, _client.Port, "Plain").UserName;
            mailMessage.From = new MailAddress(sender, "Broker Sample Application");

            return _client.SendMailAsync(mailMessage);
        }
    }
}
