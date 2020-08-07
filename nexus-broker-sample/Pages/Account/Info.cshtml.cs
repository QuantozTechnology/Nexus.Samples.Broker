using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models.Request;
using Nexus.Samples.Sdk.Models.Response;
using Nexus.Samples.Sdk.Models.Shared;

namespace Nexus.Samples.Broker.Pages.Account
{
    public class InfoModel : PageModel
    {
        private readonly NexusClient nexusClient;

        [BindProperty(SupportsGet = true)]
        [Required]
        public string Email { get; set; }

        public GetBrokerTransactionResponse[] Transactions { get; set; } = new GetBrokerTransactionResponse[] { };

        public bool? SuccessfullyProcessRequest { get; set; } = null;

        public InfoModel(NexusClient nexusClient)
        {
            this.nexusClient = nexusClient;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!string.IsNullOrWhiteSpace(Email))
            {
                var customersResult = await nexusClient.GetCustomersByEmail(Email);

                if (customersResult.IsSuccess)
                {
                    if (customersResult.Values.Total > 0)
                    {
                        var customer = customersResult.Values.Records.First();

                        var customerCode = customer.CustomerCode;

                        var transactions = await nexusClient.GetTransactions(customerCode);

                        if (!transactions.IsSuccess)
                        {
                            foreach (var error in transactions.Errors)
                            {
                                ModelState.AddModelError(nameof(Email), error);
                            }

                            return Page();
                        }

                        if (transactions.Values.Records != null)
                        {
                            Transactions = transactions.Values.Records;
                        }

                        var accountInforMail = new CreateMailRequest()
                        {
                            Type = "AccountInfoRequest",
                            References = new MailEntityCodes()
                            {
                                CustomerCode = customerCode
                            },
                            Recipient = new MailRecipient()
                            {
                                Email = Email
                            }
                        };

                        var accountInforRequest = await nexusClient.CreateMail(accountInforMail);

                        if (!accountInforRequest.IsSuccess)
                        {
                            Console.WriteLine("Failed to send AccountInfoRequest mail");
                        }
                    }
                }
            }

            SuccessfullyProcessRequest = true;

            return Page();
        }
    }
}