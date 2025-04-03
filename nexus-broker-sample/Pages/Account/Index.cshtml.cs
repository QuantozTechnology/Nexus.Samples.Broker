using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nexus.Samples.Broker.Extensions;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models.Request;
using Nexus.Samples.Sdk.Models.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Nexus.Samples.Broker.Pages.Account
{
    public class IndexModel : PageModel
    {
        private readonly NexusClient _nexusClient;
        private readonly SupportedCryptoHelper _supportedCryptoHelper;

        [Required]
        [BindProperty]
        public string Email { get; set; }

        [Required]
        [BindProperty]
        public string BankAccountNumber { get; set; }

        [Required]
        [BindProperty]
        public string BankAccountName { get; set; }

        [Required]
        [BindProperty]
        public bool IsBusiness { get; set; }

        [Required]
        [BindProperty]
        public string CountryCode { get; set; }

        [Required]
        [BindProperty]
        public string CustomerCryptoAddress { get; set; }

        [Required]
        [BindProperty]
        public string DataValue { get; set; }

        [Required]
        [BindProperty]
        public bool HasAcceptedTOS { get; set; }

        [BindProperty]
        public string CryptoCode { get; set; }

        public SelectList Countries { get; set; }

        public IndexModel(NexusClient nexusClient, SupportedCryptoHelper supportedCryptoHelper)
        {
            this._nexusClient = nexusClient;
            this._supportedCryptoHelper = supportedCryptoHelper;
        }

        public IActionResult OnGet(string crypto)
        {
            Countries = GetCountries();

            if (crypto != null)
            {
                var supportedCrypto = _supportedCryptoHelper.GetSupportedCryptoFromRoute(crypto);
                if (!supportedCrypto.IsNative)
                {
                    return Redirect($"/Account/Extra/{ supportedCrypto.Route }");
                }
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Countries = GetCountries();

            var response = await _nexusClient.CreateCustomer(new CreateCustomerRequest
            {
                CustomerCode = BankAccountNumber,
                TrustLevel = "New",
                CurrencyCode = "EUR",
                Status = "ACTIVE",
                Email = Email,
                IsBusiness = IsBusiness,
                CountryCode = CountryCode,
                Data = new Dictionary<string, string>
                {
                    { "Key1", DataValue }
                },
                BankAccounts = new BankAccount[]
                {
                    new BankAccount
                    {
                        BankAccountName = BankAccountName,
                        BankAccountNumber = BankAccountNumber
                    }
                },
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

                var createCustomerMailResponse = await _nexusClient.CreateMail(createCustomerMail);

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

                var createAccountMailResponse = await _nexusClient.CreateMail(createAccountMail);

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

        private SelectList GetCountries()
        {
            var countries = new SelectListItem[]
{
                new SelectListItem
                {
                    Text = "Netherlands",
                    Value = "NL"
                },
                new SelectListItem
                {
                    Text = "Germany",
                    Value = "DE"
                }
};

            return new SelectList(countries, "Value", "Text");
        }
    }
}