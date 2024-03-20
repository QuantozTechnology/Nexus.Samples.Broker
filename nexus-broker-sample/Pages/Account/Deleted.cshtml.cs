using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models.Request;
using Nexus.Samples.Sdk.Models.Response;
using Nexus.Samples.Sdk.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Nexus.Samples.Broker.Pages.Account
{
    public class DeletedModel : PageModel
    {
        private readonly NexusClient nexusClient;

        [BindProperty]
        [Required]
        public string AccountCode { get; set; }

        public bool? SuccessfullyProcessedRequest { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public string Code { get; set; }

        public GetAccountResponse Account { get; private set; }

        public DeletedModel(NexusClient nexusClient)
        {
            this.nexusClient = nexusClient;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

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

            var deleteAccountRequest = await nexusClient.DeleteAccount(AccountCode);

            if (deleteAccountRequest.IsSuccess)
            {
                SuccessfullyProcessedRequest = true;

                var accountResponse = await nexusClient.GetAccount(AccountCode);

                if (accountResponse.IsSuccess)
                {
                    var customerResponse = await nexusClient.GetCustomer(accountResponse.Values.CustomerCode);

                    if (customerResponse.IsSuccess)
                    {
                        var activateAccountMail = new CreateMailRequest()
                        {
                            Type = "AccountDeletedByRequest",
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

                        var deleteAccountMailResponse = await nexusClient.CreateMail(activateAccountMail);

                        if (!deleteAccountMailResponse.IsSuccess)
                        {
                            Console.WriteLine("Failed to send AccountDeletedByRequest mail");
                        }
                    }
                }

                Account = accountResponse.Values;

                return Page();
            }
            else
            {
                foreach (var error in deleteAccountRequest.Errors)
                {
                    switch (error)
                    {
                        case "AccountNotFound": return NotFound();
                        default:
                            break;
                    }
                }

                SuccessfullyProcessedRequest = false;

                return Page();
            }
        }
    }
}