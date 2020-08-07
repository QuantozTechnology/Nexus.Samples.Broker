using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models.Request;
using Nexus.Samples.Sdk.Models.Shared;

namespace Nexus.Samples.Broker.Pages.Account
{
    public class DeleteModel : PageModel
    {
        private readonly NexusClient nexusClient;

        [BindProperty]
        [Required]
        public string AccountCode { get; set; }

        [BindProperty]
        [Required]
        public string Email { get; set; }

        public bool? SuccessfullyProcessRequest { get; set; } = null;

        public DeleteModel(NexusClient nexusClient)
        {
            this.nexusClient = nexusClient;
        }


        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var accountResponse = await nexusClient.GetAccount(AccountCode);

            if (accountResponse.IsSuccess)
            {
                var customerResponse = await nexusClient.GetCustomer(accountResponse.Values.CustomerCode);

                if (customerResponse.IsSuccess 
                    && Email == customerResponse.Values.Email
                    && accountResponse.Values.AccountStatus == "ACTIVE")
                {
                    var deletedAccountMail = new CreateMailRequest()
                    {
                        Type = "AccountDeleteRequested",
                        References = new MailEntityCodes()
                        {
                            AccountCode = AccountCode
                        },
                        Recipient = new MailRecipient()
                        {
                            Email = Email
                        }
                    };

                    var mailSentResponse = await nexusClient.CreateMail(deletedAccountMail);

                    if (!mailSentResponse.IsSuccess)
                    {
                        SuccessfullyProcessRequest = false;
                    }
                }
            }

            SuccessfullyProcessRequest = true;

            return Page();
        }
    }
}