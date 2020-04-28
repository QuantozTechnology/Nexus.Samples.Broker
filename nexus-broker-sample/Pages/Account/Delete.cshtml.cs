using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nexus.Samples.Sdk;

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

                return Page();
            }

            return RedirectToPage("/Account/Deleted");
        }
    }
}