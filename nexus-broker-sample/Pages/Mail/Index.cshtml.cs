using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nexus.Samples.MailClient;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models.Response;

namespace Nexus.Samples.Broker
{
    public class IndexModel : PageModel
    {
        private readonly NexusClient nexusClient;
        private readonly NexusMailClient mailClient;

        public GetMailResponse[] Mails { get; set; } = new GetMailResponse[] { };

        public bool Successful { get; set; }

        public IndexModel(NexusClient nexusClient, NexusMailClient mailClient)
        {
            this.nexusClient = nexusClient;
            this.mailClient = mailClient;
        }

        public async Task OnGet()
        {
            var response = await nexusClient.GetReadyToSendMails(1, String.Join("|", NexusMailClient.SupportMailTypes));

            if (response.IsSuccess)
            {
                if (response.Values.Records != null)
                {
                    Mails = response.Values.Records;
                }
                Successful = true;
            }
        }

        public async Task OnPostSend()
        {
            await mailClient.SendMailsAsync();

            await OnGet();
        }
    }
}