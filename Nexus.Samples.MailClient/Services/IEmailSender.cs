using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nexus.Samples.MailClient
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, List<string> CC, List<string> BCC, string subject, string message);
    }
}
