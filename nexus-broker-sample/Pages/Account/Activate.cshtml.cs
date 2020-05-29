using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models.Request;
using Nexus.Samples.Sdk.Models.Shared;

namespace Nexus.Samples.Broker.Pages.Account
{
    public class ActivateModel : PageModel
    {
        private readonly NexusClient nexusClient;

        [BindProperty(SupportsGet = true)]
        public string Code { get; set; }

        public string AccountCode { get; set; }

        public bool Successful { get; set; }

        public ActivateModel(NexusClient nexusClient)
        {
            this.nexusClient = nexusClient;
        }

        public async Task<IActionResult> OnGet()
        {
            var mailResponse = await nexusClient.GetMailByCode(Code);

            if (mailResponse.IsSuccess)
            {
                var mail = mailResponse.Values;
                AccountCode = mail.References.AccountCode;
            }
            else
            {
                AccountCode = Code;
            }

            var activateAccountResponse = await nexusClient.ActivateAccount(AccountCode);

            if (activateAccountResponse.IsSuccess)
            {
                Successful = true;

                var accountResponse = await nexusClient.GetAccount(AccountCode);

                if (accountResponse.IsSuccess)
                {
                    var customerResponse = await nexusClient.GetCustomer(accountResponse.Values.CustomerCode);

                    if (customerResponse.IsSuccess)
                    {
                        var activateAccountMail = new CreateMailRequest()
                        {
                            Type = "NewAccountActivated",
                            References = new MailEntityCodes()
                            {
                                AccountCode = AccountCode,
                                CustomerCode = customerResponse.Values.CustomerCode
                            },
                            Recipient = new MailRecipient()
                            {
                                Email = customerResponse.Values.Email
                            }
                        };

                        var activateAccountMailResponse = await nexusClient.CreateMail(activateAccountMail);

                        if (!activateAccountMailResponse.IsSuccess)
                        {
                            Console.WriteLine("Failed to send NewAccountActivated mail");
                        }
                    }
                }

                return Page();
            }
            else
            {
                foreach (var error in activateAccountResponse.Errors)
                {
                    switch (error)
                    {
                        case "AccountNotFound": return NotFound();
                        default:
                            break;
                    }
                }

                Successful = false;

                return Page();
            }
        }
    }
}