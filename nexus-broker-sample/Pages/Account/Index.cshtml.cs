using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models.Request;
using Nexus.Samples.Sdk.Models.Shared;

namespace Nexus.Samples.Broker.Pages.Account
{
    [BindProperties]
    public class IndexModel : PageModel
    {
        private readonly NexusClient nexusClient;

        [Required]
        public string Email { get; set; }

        [Required]
        public string BankAccountNumber { get; set; }

        [Required]
        public string BankAccountName { get; set; }

        [Required]
        public string CustomerCryptoAddress { get; set; }

        [Required]
        public bool HasAcceptedTOS { get; set; }
        public string CryptoCode { get; set; }

        public IndexModel(NexusClient nexusClient)
        {
            this.nexusClient = nexusClient;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = await nexusClient.CreateCustomer(new CreateCustomerRequest
            {
                CustomerCode = BankAccountNumber,
                TrustLevel = "New",
                CurrencyCode = "EUR",
                Status = "ACTIVE",
                Email = Email,
                Accounts = new Sdk.Models.Request.Account[]
                {
                    new Sdk.Models.Request.Account
                    {
                        AccountType = "BROKER",
                        CryptoCode = CryptoCode,
                        CustomerCryptoAddress = CustomerCryptoAddress
                    }
                }
            });

            if (response.IsSuccess)
            {
                var createCustomerMail = new CreateMailRequest()
                {
                    Type = "NewCustomerRequested",
                    References = new MailEntityCodes()
                    {
                        CustomerCode = BankAccountNumber
                    },
                    Recipient = new MailRecipient()
                    {
                        Email = Email
                    }
                };

                var createCustomerMailResponse = await nexusClient.CreateMail(createCustomerMail);

                var accountCode = response.Values.Accounts[0].AccountCode;

                var createAccountMail = new CreateMailRequest()
                {
                    Type = "NewAccountRequested",
                    References = new MailEntityCodes()
                    {
                        AccountCode = accountCode
                    },
                    Recipient = new MailRecipient()
                    {
                        Email = Email
                    }
                };

                var createAccountMailResponse = await nexusClient.CreateMail(createAccountMail);

                return RedirectToPage("/Account/Created", new { accountCode });
            }
            else
            {
                foreach (var error in response.Errors)
                {
                    switch (error)
                    {
                        case "CustomerCodeRequired": ModelState.AddModelError(nameof(BankAccountNumber), "Required"); break;
                        default:
                            break;
                    }
                }
            }

            return Page();
        }
    }
}