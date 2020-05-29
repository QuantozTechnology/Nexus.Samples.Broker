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

            CreateMailRequest deletedAccountMail = null;

            var accountResponse = await nexusClient.GetAccount(AccountCode);

            if (accountResponse.IsSuccess)
            {
                var customerResponse = await nexusClient.GetCustomer(accountResponse.Values.CustomerCode);

                if (customerResponse.IsSuccess)
                {
                    deletedAccountMail = new CreateMailRequest()
                    {
                        Type = "AccountDeletedByRequest",
                        References = new MailEntityCodes()
                        {
                            AccountCode = AccountCode
                        },
                        Recipient = new MailRecipient()
                        {
                            Email = customerResponse.Values.Email
                        }
                    };
                }
            }

            var deleteResult = await nexusClient.DeleteAccount(AccountCode);

            if (!deleteResult.IsSuccess)
            {
                foreach (var error in deleteResult.Errors)
                {
                    switch (error)
                    {
                        case "AccountNotFound": ModelState.AddModelError(nameof(AccountCode), "Account not found."); break;
                        default:
                            break;
                    }
                }

                return BadRequest(deleteResult.Values.ToString());
            }

            if (deletedAccountMail != null)
            {
                var deletedAccountMailResponse = await nexusClient.CreateMail(deletedAccountMail);
            }

            return RedirectToPage("/Account/Deleted", new { AccountCode });
        }
    }
}