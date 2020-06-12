using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Nexus.Samples.MailClient;

namespace MailService
{
    public class MailFunctions
    {
        private readonly NexusMailClient _mailClient;

        public MailFunctions(NexusMailClient mailClient)
        {
            _mailClient = mailClient;
        }

        [FunctionName("SendAll")]
        public async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                await _mailClient.SendMailsAsync();
            }
            catch (Exception ex)
            {
                log.LogInformation($"An exception occured at: {DateTime.Now} with the following message: {ex.Message}");
            }
        }
    }
}
