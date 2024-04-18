using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nexus.Samples.Broker.Extensions;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models;
using Nexus.Samples.Sdk.Models.Request;
using QRCoder;
using System.Threading.Tasks;

namespace Nexus.Samples.Broker.Pages.Sell
{
    public class InitiateModel : PageModel
    {
        private readonly NexusClient _nexusClient;
        private readonly SupportedCryptoHelper _supportedCryptoHelper;

        public AccountSellResponsePT InitiateSellModel { get; set; }

        [BindProperty]
        public AccountSellPT SellModel { get; set; }

        public InitiateModel(NexusClient nexusClient, SupportedCryptoHelper supportedCryptoHelper)
        {
            this._nexusClient = nexusClient;
            this._supportedCryptoHelper = supportedCryptoHelper;
        }

        public async Task<IActionResult> OnPost()
        {
            if (string.IsNullOrWhiteSpace(SellModel.AccountCode) == true)
            {
                return RedirectToAction("Index", "Sell");
            }

            var accountResponse = await this._nexusClient.GetAccount(SellModel.AccountCode.Trim().ToUpper());
            if (!accountResponse.IsSuccess)
            {
                return BadRequest(accountResponse.Errors);
            }
            var account = accountResponse.Values;

            var customerResponse = await this._nexusClient.GetCustomer(account.CustomerCode);
            if (!customerResponse.IsSuccess)
            {
                return BadRequest(customerResponse.Errors);
            }
            var customer = customerResponse.Values;

            var paymentMethodCode = _supportedCryptoHelper.GetSupportedCrypto(account.DcCode).SellPaymentMethodCode;

            var initiateBrokerSellRequest = new InitiateBrokerSellRequest() 
            {
                AccountCode = account.AccountCode,
                CryptoAmount = SellModel.CryptoAmount,
                IP = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                PaymentMethodCode = paymentMethodCode
            };

            var initiateBrokerSellResponse = await this._nexusClient.InitiateBrokerSell(initiateBrokerSellRequest);
            if (!initiateBrokerSellResponse.IsSuccess)
            {
                return BadRequest(initiateBrokerSellResponse.Errors);
            }
            var initiatedBrokerSell = initiateBrokerSellResponse.Values;

            var transactionResponse = await this._nexusClient.GetTransaction(initiatedBrokerSell.TransactionCode);
            if (!transactionResponse.IsSuccess)
            {
                return BadRequest(transactionResponse.Errors);
            }
            var transaction = transactionResponse.Values;

            var qrGenerator = new QRCodeGenerator();
            string Generate(string value)
            {
                var qrCodeData = qrGenerator.CreateQrCode(value, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new SvgQRCode(qrCodeData);
                return qrCode.GetGraphic(10);
            }
            
            var qrCode = initiatedBrokerSell.CryptoAddress.Trim();

            if (account.DcCode != "XLM" && account.DcCode != "ETH")
            {
                string amount = SellModel.CryptoAmount.ToString().Replace(",", ".");

                qrCode += "?amount=" + amount + "%26label=" + transaction.TransactionCode;
                if (account.DcCode == "BTC")
                {
                    qrCode = "bitcoin:" + qrCode;
                }
            }

            ViewData["QRCode"] = Generate(qrCode);
            ViewData["QRCodeData"] = qrCode;
            ViewData["CryptoName"] = GetCryptoName(account.DcCode);

            InitiateSellModel = new AccountSellResponsePT()
            {
                AccountCode = account.AccountCode,
                AfterFee = initiatedBrokerSell.ValueInFiatAfterFees,
                BankAccountNumber = customer.BankAccount,
                BtcAddress = initiatedBrokerSell.CryptoAddress,
                BtcAmount = SellModel.CryptoAmount,
                Currency = initiatedBrokerSell.CurrencyCode,
                Email = customer.Email,
                TransactionCode = initiatedBrokerSell.TransactionCode,
                TransactionTimestamp = transaction.Created
            };

            return Page();
        }

        private string GetCryptoName(string cryptoCode)
        {
            return _supportedCryptoHelper.GetSupportedCrypto(cryptoCode).Name;
        }
    }
}