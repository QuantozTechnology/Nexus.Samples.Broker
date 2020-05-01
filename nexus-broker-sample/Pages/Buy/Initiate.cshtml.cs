using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nexus.Samples.Broker.API;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models;

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

            var req = new
            {
                AccountCode = BuyModel.AccountCode,
                //IP = Request.UserHostAddress,
                IP = "::1",
                Currency = BuyModel.Currency
            };

            HttpResponseMessage createResponse = await nexusClient.PostRequestAsync("api/Buy/Check/", req);

            if (!createResponse.IsSuccessStatusCode)
            {
                //HttpError responseContent = await createResponse.Content.ReadAsAsync<HttpError>();

                //ExtractModelState(responseContent, "BUYERROR");

                //return await Index(model);
                return BadRequest();
            }

            BuyInfoPT buyInfo = await createResponse.Content.ReadAsAsync<BuyInfoPT>();

            // Check if bank is still unknown, if so: open Bank Information Form ...
            //if (buyInfo.AccountValid && buyInfo.IsUnknownBank)
            //{
            //    return RedirectToAction("BankInfo", new { id = req.AccountCode });
            //}

            //AccountInfo account = await nexusClient.GetRequestAsync<AccountInfo>("api/Account/Get/", BuyModel.AccountCode);

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

            var accountBuyPT = new
            {
                AccountCode = BuyModel.AccountCode,
                Amount = BuyModel.Amount,
                Currency = BuyModel.Currency,
                //IP = Request.UserHostAddress,
                IP = "::1",

                PaymentMethodCode = BuyModel.PaymentMethodCode
            };

            HttpResponseMessage response = await nexusClient.PostRequestAsync("api/Buy/Create", accountBuyPT);

            if (!response.IsSuccessStatusCode)
            {
                //HttpError responseContent = await response.Content.ReadAsAsync<HttpError>();

                //ExtractModelState(responseContent, "BUYERROR");

                //return await Index(model);
                return BadRequest();
            }

            string transactionCode = await response.Content.ReadAsAsync<string>();

            PSPInfoPT pspinfo = await nexusClient.GetRequestAsync<PSPInfoPT>($"transaction/{transactionCode}/pspinfo");

            var priceSummaryResponse = await nexusClient.SimulateBuyBroker(new Sdk.Models.Request.SimulateBuyBrokerRequest
            {
                AccountCode = BuyModel.AccountCode,
                Amount = (decimal)BuyModel.Amount,
                Currency = BuyModel.Currency,
                PaymentMethodCode = BuyModel.PaymentMethodCode
            });

            if (!priceSummaryResponse.IsSuccess)
            {
                return BadRequest();
            }

            InitiateBuyModel = new InitiateBuyModel()
            {
                PaymentMethodName = pspinfo.PayMethodName,
                PaymentMethodCode = pspinfo.PayMethodCode,
                AccountCode = BuyModel.AccountCode,
                TransactionCode = transactionCode,
                TransactionTimestamp = pspinfo.TransactionTimestamp,
                TransactionFixedMinutes = pspinfo.TransactionFixedMinutes,
                Email = customer.Email, // obfuscate
                BtcAddress = account.CustomerCryptoAddress,
                //IBAN = customer.IBAN, // obfuscate
                IBAN = customer.CustomerCode, // obfuscate
                Amount = BuyModel.Amount.ToString("F2"),
                EstimateBTC = priceSummaryResponse.Values.CryptoAmount.ToString("F8"),
                PaymentMethodLogoURL = "/img/psp/" + pspinfo.PayMethodCode.Trim() + "_BIG.jpg",
                PaymentMethodIconURL = "/img/psp/" + pspinfo.PayMethodCode.Trim() + ".png",
                ActionUrl = pspinfo.URLPSPInitiateFull.Trim(),
                Currency = priceSummaryResponse.Values.CurrencyCode,
                SendDelayHours = pspinfo.SendDelayHours,
                DCCode = buyInfo.DCCode
            };

            // Special SEPA method info ...
            if ((pspinfo.PayMethodCode.Trim().ToUpper() == "SEPA")
                || (pspinfo.PayMethodCode.Trim().ToUpper() == "SWISS_BANK"))
            {
                InitiateBuyModel.BankName = pspinfo.SEPAName;
                InitiateBuyModel.BankIBAN = pspinfo.SEPAIBAN;
                InitiateBuyModel.BankBIC = pspinfo.SEPABIC;
                InitiateBuyModel.BankAddress = pspinfo.SEPAAddress;
            }

            return Page();
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

        public string ActionUrl { get; set; }

        public string Currency { get; set; }

        public int SendDelayHours { get; set; }

        public string DCCode { get; set; }


        public string BankName { get; set; }
        public string BankIBAN { get; set; }
        public string BankBIC { get; set; }
        public string BankAddress { get; set; }
    }

    public class PSPInfoPT
    {
        public string PayMethodName { get; set; }
        public string PayMethodCode { get; set; }
        public string URLPostResult { get; set; }
        public string URLReturnSuccess { get; set; }
        public string URLReturnCancel { get; set; }
        public string URLReturnFailure { get; set; }
        public string URLReturnPending { get; set; }
        public string URLPSPInitiateFull { get; set; }
        public string InfoData { get; set; }
        public string SealData { get; set; }

        public string TransactionCode { get; set; }
        public DateTime TransactionTimestamp { get; set; }
        public int TransactionFixedMinutes { get; set; }
        public int SendDelayHours { get; set; }

        #region SEPAinfo
        public string SEPAName { get; set; }
        public string SEPAIBAN { get; set; }
        public string SEPABIC { get; set; }
        public string SEPAAddress { get; set; }
        #endregion

    }
}