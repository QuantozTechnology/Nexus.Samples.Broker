using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Nexus.Samples.MailClient;

namespace FunctionApp1
{
    public class MailFunctions
    {
        private readonly ILogger _logger;
        private readonly NexusMailClient _mailClient;

        public MailFunctions(ILoggerFactory loggerFactory, NexusMailClient mailClient)
        {
            _logger = loggerFactory.CreateLogger<MailFunctions>();
            _mailClient = mailClient;
        }

        [Function("SendAll")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"Mail send function executed at: {DateTime.Now}");

            try
            {
                await _mailClient.SendMailsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"An exception occured at: {DateTime.Now} with the following message: {ex.Message}");
            }
        }
    }
}
