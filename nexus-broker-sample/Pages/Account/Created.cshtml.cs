using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models.Response;
using System.Threading.Tasks;

namespace Nexus.Samples.Broker.Pages.Account
{
    public class CreatedModel : PageModel
    {
        private readonly NexusClient nexusClient;

        [BindProperty(SupportsGet = true)]
        public string AccountCode { get; set; }

        public GetAccountResponse AccountResponse { get; private set; }

        public CreatedModel(NexusClient nexusClient)
        {
            this.nexusClient = nexusClient;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var accountResponse = await nexusClient.GetAccount(AccountCode);

            if (accountResponse.IsSuccess)
            {
                AccountResponse = accountResponse.Values;

                return Page();
            }
            else
            {
                foreach (var error in accountResponse.Errors)
                {
                    switch (error)
                    {
                        case "AccountNotFound": return NotFound();
                        default: return Page();
                    }
                }

                return Page();
            }
        }
    }
}