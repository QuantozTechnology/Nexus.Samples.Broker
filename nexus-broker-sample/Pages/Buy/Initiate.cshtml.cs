using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models;
using Nexus.Samples.Sdk.Models.Request;
using System;
using System.Threading.Tasks;

namespace Nexus.Samples.Broker.Pages.Buy
{
    public class InitiateModel : PageModel
    {
        private readonly NexusClient nexusClient;

        public InitiateBuyModel InitiateBuyModel { get; set; }

        [BindProperty]
        public BuyModel BuyModel { get; set; }

        public InitiateModel(NexusClient nexusClient)
        {
            this.nexusClient = nexusClient;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(BuyModel.AccountCode))
            {
                return RedirectToPage("Index");
            }

            BuyModel.AccountCode = BuyModel.AccountCode.Trim().ToUpper();

            var accountResult = await nexusClient.GetAccount(BuyModel.AccountCode);

            if (!accountResult.IsSuccess)
            {
                return BadRequest();
            }

            var account = accountResult.Values;

            var customerResult = await nexusClient.GetCustomer(accountResult.Values.CustomerCode);

            if (!customerResult.IsSuccess)
            {
                return BadRequest();
            }

            var customer = customerResult.Values;

            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            var initiateBrokerBuyRequest = new InitiateBrokerBuyRequest()
            {
                AccountCode = BuyModel.AccountCode,
                Amount = (decimal) BuyModel.Amount,
                Currency = BuyModel.Currency,
                PaymentMethodCode = BuyModel.PaymentMethodCode,
                IP = ipAddress
            };
            var initiateBuyResponse = await nexusClient.InitiateBrokerBuy(initiateBrokerBuyRequest);
            if (!initiateBuyResponse.IsSuccess)
            {
                return BadRequest(initiateBuyResponse.Errors);
            }
            var initiateBuyTransaction = initiateBuyResponse.Values;

            var priceSummaryResponse = await nexusClient.SimulateBuyBroker(new SimulateBuyBrokerRequest
            {
                AccountCode = BuyModel.AccountCode,
                Amount = (decimal)BuyModel.Amount,
                Currency = BuyModel.Currency,
                PaymentMethodCode = BuyModel.PaymentMethodCode
            });

            if (!priceSummaryResponse.IsSuccess)
            {
                return BadRequest(priceSummaryResponse.Errors);
            }

            var transactionResponse = await nexusClient.GetTransaction(initiateBuyTransaction.TransactionCode);
            if (!transactionResponse.IsSuccess)
            {
                return BadRequest(transactionResponse.Errors);
            }
            var transaction = transactionResponse.Values;
            var imageBaseUrl = GetImageBaseURL(initiateBuyTransaction.PaymentMethodInfo.PaymentMethodCode.ToUpper());
            InitiateBuyModel = new InitiateBuyModel()
            {
                PaymentMethodName = initiateBuyTransaction.PaymentMethodInfo.PaymentMethodName,
                PaymentMethodCode = initiateBuyTransaction.PaymentMethodInfo.PaymentMethodCode,
                AccountCode = BuyModel.AccountCode,
                TransactionCode = initiateBuyTransaction.TransactionCode,
                TransactionTimestamp = DateTime.Parse(initiateBuyTransaction.Created),
                TransactionFixedMinutes = initiateBuyTransaction.PaymentMethodInfo.TransactionFixedInMinutes,
                Email = customer.Email, // obfuscate
                BtcAddress = account.CustomerCryptoAddress,
                IBAN = customer.BankAccount, // obfuscate
                Amount = BuyModel.Amount.ToString("F2"),
                EstimateBTC = priceSummaryResponse.Values.CryptoAmount.ToString("F8"),
                PaymentMethodLogoURL = imageBaseUrl + "_BIG.jpg",
                PaymentMethodIconURL = imageBaseUrl + ".png",
                Currency = priceSummaryResponse.Values.CurrencyCode,
                DCCode = transaction.CryptoCurrencyCode
            };

            return Page();
        }

        private string GetImageBaseURL(string paymentMethodCode)
        {
            var imageUrl = "/img/psp/";
            if (paymentMethodCode.Contains("SEPA"))
            {
                imageUrl += "SEPA";
            }
            else if (paymentMethodCode.Contains("SOFORT"))
            {
                imageUrl += "SOFORT_SOFORT";
            }
            else if (paymentMethodCode.Contains("IDEAL"))
            {
                imageUrl += "IDEAL_PAYNL";
            }
            else if (paymentMethodCode.Contains("GIROPAY"))
            {
                imageUrl += "GIROPAY_PAYNL";
            }
            else if (paymentMethodCode.Contains("INTERAC"))
            {
                imageUrl += "INTERAC_SALT";
            }
            else if (paymentMethodCode.Contains("MRCASH"))
            {
                imageUrl += "MRCASH_PAYNL";
            }
            else if (paymentMethodCode.Contains("MYBANK"))
            {
                imageUrl += "MYBANK_PAYNL";
            }
            else if (paymentMethodCode.Contains("SKRILL"))
            {
                imageUrl += "SKRILL_SKRILL";
            }
            else if (paymentMethodCode.Contains("SWISS_BANK"))
            {
                imageUrl += "SWISS_BANK";
            }
            else
            {
                imageUrl += "NOT_SUPPORTED";
            }
            return imageUrl;
        }
    }

    public class InitiateBuyModel
    {
        public string PaymentMethodName { get; set; }

        public string PaymentMethodCode { get; set; }

        public string AccountCode { get; set; }

        public string TransactionCode { get; set; }

        public DateTime TransactionTimestamp { get; set; }

        public int TransactionFixedMinutes { get; set; }

        public string Email { get; set; }

        public string BtcAddress { get; set; }

        public string IBAN { get; set; }

        public string BicCode { get; set; }

        public string Amount { get; set; }

        public string EstimateBTC { get; set; }

        public string PaymentMethodLogoURL { get; set; }

        public string PaymentMethodIconURL { get; set; }

        public string Currency { get; set; }

        public string DCCode { get; set; }
    }
}