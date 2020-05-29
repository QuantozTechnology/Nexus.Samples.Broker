using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models.Response;

namespace Nexus.Samples.Broker.Pages.Account
{
    public class DeletedModel : PageModel
    {
        private readonly NexusClient nexusClient;

        [BindProperty]
        [Required]
        public string AccountCode { get; set; }

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

            var accountResult = await nexusClient.GetAccount(AccountCode);

            if (!accountResult.IsSuccess)
            {
                return BadRequest();
            }

            Account = accountResult.Values;

            return Page();
        }
    }
}