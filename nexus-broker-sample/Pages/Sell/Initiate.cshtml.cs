using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models;
using QRCoder;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nexus.Samples.Broker.Pages.Sell
{
    public class InitiateModel : PageModel
    {
        private readonly NexusClient nexusClient;

        public AccountSellResponsePT InitiateSellModel { get; set; }

        [BindProperty]
        public AccountSellPT SellModel { get; set; }

        public InitiateModel(NexusClient nexusClient)
        {
            this.nexusClient = nexusClient;
        }

        public async Task<IActionResult> OnPost()
        {
            if (string.IsNullOrWhiteSpace(SellModel.AccountCode) == true)
            {
                return RedirectToAction("Index", "Sell");
            }

            SellModel.AccountCode = SellModel.AccountCode.Trim().ToUpper();
            //SellModel.IP = Request.UserHostAddress;
            SellModel.Ip = "::1";
            SellModel.Currency = SellModel.Currency?.Trim().ToUpper();

            var response = await nexusClient.PostRequestAsync("api/Sell/Post", SellModel);

            if (!response.IsSuccessStatusCode)
            {
                string message = await response.Content.ReadAsStringAsync();

                //HttpError responseContent = JsonConvert.DeserializeObject<HttpError>(message);

                //ExtractModelState(responseContent, "SELLERROR");

                //SellInfoRequestPT req = new SellInfoRequestPT()
                //{
                //    AccountCode = model.AccountCode,
                //    IP = model.IP
                //};

                //SellInfoPT sellInfo = await nexusClient.PostAndGetRequestAsync<SellInfoPT, SellInfoRequestPT>("api/Sell/PostProcessInformation/", req);

                //model.BTCstr = "0";
                //model.Currency = sellInfo.Currency;

                //ViewBag.SellServiceAvailable = sellInfo.SellServiceAvailable;

                //ViewBag.PromoSettings = await PassthroughClient.GetRequestAsync<PromoSettingsPT>("api/Account/GetPromoSettings");
                //ViewBag.Currency = model.Currency;

                //return View("Index", model);
            }

            InitiateSellModel = await response.Content.ReadAsAsync<AccountSellResponsePT>();

            if (InitiateSellModel.NeedAddressDetails)
            {
                return RedirectToAction("UpdateAddress", "Account", new { id = SellModel.AccountCode });
            }

            var qrGenerator = new QRCodeGenerator();

            string Generate(string value)
            {
                var qrCodeData = qrGenerator.CreateQrCode(value, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new SvgQRCode(qrCodeData);
                return qrCode.GetGraphic(10);
            }

            var qrCode = InitiateSellModel.BtcAddress.Trim();

            if (SellModel.CryptoCode != "XLM" && SellModel.CryptoCode != "ETH")
            {
                string amount = InitiateSellModel.BtcAmount.ToString().Replace(",", ".");

                qrCode += "?amount=" + amount + "%26label=" + InitiateSellModel.TransactionCode.Trim();

                if (SellModel.CryptoCode == "BTC")
                {
                    qrCode = "bitcoin:" + qrCode;
                }
            }

            ViewData["QRCode"] = Generate(qrCode);
            ViewData["QRCodeData"] = qrCode;

            return Page();
        }
    }
}