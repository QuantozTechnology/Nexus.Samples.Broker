using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models.Response;

namespace Nexus.Samples.Broker.Pages.Account
{
    public class InfoModel : PageModel
    {
        private readonly NexusClient nexusClient;

        [BindProperty(SupportsGet = true)]
        public string AccountCode { get; set; }

        public GetBrokerTransactionResponse[] Transactions { get; set; } = new GetBrokerTransactionResponse[] { };

        public InfoModel(NexusClient nexusClient)
        {
            this.nexusClient = nexusClient;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!string.IsNullOrWhiteSpace(AccountCode))
            {
                var account = await nexusClient.GetAccount(AccountCode);

                var transactions = await nexusClient.GetTransactions(account.Values.CustomerCode);

                Transactions = transactions.Values.Records;
            }

            return Page();
        }
    }
}