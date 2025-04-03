using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Nexus.Samples.Broker.Extensions;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models.Request;
using Nexus.Samples.Sdk.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Nexus.Samples.Broker.Pages.Account
{
    [BindProperties]
    public class ExtraModel : PageModel
    {
        private readonly NexusClient _nexusClient;
        private readonly ILogger<ExtraModel> _logger;
        private readonly SupportedCryptoHelper _supportedCryptoHelper;

        [Required]
        public string AccountCode { get; set; }

        [Required]
        public string CustomerCryptoAddress { get; set; }

        public string CryptoCode { get; set; }

        public ExtraModel(NexusClient nexusClient, ILogger<ExtraModel> logger, SupportedCryptoHelper supportedCryptoHelper)
        {
            this._nexusClient = nexusClient;
            this._logger = logger;
            this._supportedCryptoHelper = supportedCryptoHelper;
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

            var getAccountResponse = await _nexusClient.GetAccount(AccountCode);

            if (!getAccountResponse.IsSuccess)
            {
                foreach (var error in getAccountResponse.Errors)
                {
                    _logger.LogInformation(error);
                    switch (error)
                    {
                        case "AccountNotFound": ModelState.AddModelError(nameof(AccountCode), "Account not found."); break;
                        default:
                            break;
                    }
                }

                return Page();
            }

            var supportedCrypto = _supportedCryptoHelper.GetSupportedCrypto(CryptoCode);
            if (!supportedCrypto.IsNative)
            {
                // Do extra validation for non native cryptos
                if (!string.Equals(supportedCrypto.DependendNativeCrypto.ToLower(), getAccountResponse.Values.DcCode.ToLower())) {
                    var dependendCrypto = _supportedCryptoHelper.GetSupportedCrypto(supportedCrypto.DependendNativeCrypto);
                    ModelState.AddModelError(nameof(AccountCode), $"Account not a valid {dependendCrypto.Name} account");
                    return Page();
                }
            }

            var response = await _nexusClient.CreateAccount(getAccountResponse.Values.CustomerCode, new CreateAccountRequest
            {
                AccountType = CreateAccountRequestAccountType.BROKER,
                CryptoCode = CryptoCode,
                CustomerCryptoAddress = CustomerCryptoAddress,
                IP = "::1"
            });

            if (response.IsSuccess)
            {
                var customerResponse = await _nexusClient.GetCustomer(getAccountResponse.Values.CustomerCode);

                if (customerResponse.IsSuccess)
                {
                    var createAccountMail = new CreateMailRequest()
                    {
                        Type = "NewAccountRequested",
                        References = new MailEntityCodes()
                        {
                            AccountCode = response.Values.AccountCode
                        },
                        Recipient = new MailRecipient()
                        {
                            Email = customerResponse.Values.Email
                        }
                    };

                    await _nexusClient.CreateMail(createAccountMail);
                }

                return RedirectToPage("/Account/Created", new { response.Values.AccountCode});
            }
            else
            {
                foreach (var error in response.Errors)
                {
                    _logger.LogInformation(error);
                    switch (error)
                    {
                        case "ERRORMESSAGE007": ModelState.AddModelError(nameof(CustomerCryptoAddress), "Invalid address."); break;
                        default:
                            break;
                    }
                }
            }

            return Page();
        }
    }
}