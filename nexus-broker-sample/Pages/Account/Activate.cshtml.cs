using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nexus.Samples.Sdk;

namespace Nexus.Samples.Broker.Pages.Account
{
    public class ActivateModel : PageModel
    {
        private readonly NexusClient nexusClient;

        [BindProperty(SupportsGet = true)]
        public string AccountCode { get; set; }

        public bool Successful { get; set; }

        public ActivateModel(NexusClient nexusClient)
        {
            this.nexusClient = nexusClient;
        }

        public async Task<IActionResult> OnGet()
        {
            var activateAccountResponse = await nexusClient.ActivateAccount(AccountCode);

            if (activateAccountResponse.IsSuccess)
            {
                Successful = true;

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