using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models;
using Nexus.Samples.Sdk.Models.Request;
using Nexus.Samples.Sdk.Models.Response;

namespace Nexus.Samples.Broker.API
{
    [Route("api/ajax")]
    [ApiController]
    public class AjaxController : ControllerBase
    {
        private readonly NexusClient nexusClient;

        public AjaxController(NexusClient nexusClient)
        {
            this.nexusClient = nexusClient;
        }

        [HttpGet("getprices")]
        public async Task<ActionResult<MarketPricesModel>> GetPrices([FromQuery]string currency)
        {
            //try
            //{
            //    ValidateRequestHeader(Request);
            //}
            //catch (System.Web.Mvc.HttpAntiForgeryException)
            //{
            //    return null;
            //}

            //var req = new
            //{
            //    Currency = currency
            //};

            //PricesPT response = await nexusClient.PostAndGetRequestAsync<PricesPT, object>("api/BitcoinPrice/PostGetPrices/", req);

            var getPricesResponse = await nexusClient.GetPrices(currency);

            if (!getPricesResponse.IsSuccess)
            {
                return BadRequest();
            }

            MarketPricesModel model = MarketPricesModel.CreateFromPT(getPricesResponse.Values);

            return model;
        }

        [HttpGet("checkbuyinfo/{id?}")]
        public async Task<IActionResult> CheckBuyInfo(string id = null)
        {
            //try
            //{
            //    ValidateRequestHeader(Request);
            //}
            //catch (System.Web.Mvc.HttpAntiForgeryException)
            //{
            //    return null;
            //}

            var model = new
            {
                AccountCode = id,
                //IP = HttpContext.Current.Request.UserHostAddress
                IP = "::1"
            };

            //var cookies = Request.Headers.GetCookies("currency");

            //if (cookies != null && cookies.Any())
            //{
            //    var cookie = cookies.First().Cookies.SingleOrDefault(t => t.Name == "currency");

            //    if (cookie != null)
            //    {
            //        model.Currency = cookie.Value;
            //    }
            //}

            HttpResponseMessage response = await nexusClient.PostRequestAsync("api/Buy/Check/", model);

            if (response.IsSuccessStatusCode)
            {
                var info = await response.Content.ReadAsAsync<BuyInfoPT>();

                if (info.LimitReasons != null)
                {
                    //only include limit reasons that are handled by the front end
                    info.LimitReasons =
                        info.LimitReasons
                        .Where(lr =>
                            lr == "DailyBuyLimit"
                            || lr == "MonthlyBuyLimit"
                            || lr == "LowBalance")
                        .OrderBy(lr => lr)
                        .ToArray();
                }

                return Ok(info);
            }
            else
            {
                //var error = await response.Content.ReadAsAsync<HttpError>();

                //Trace.WriteLine(error.Message + Environment.NewLine + error.StackTrace);

                return BadRequest();
            }
        }

        [HttpPost("bitcoinvalue")]
        public async Task<ActionResult<SimulateBuyBrokerResponse>> BitcoinValue([FromBody]SimulateBuyBrokerRequest value)
        {
            //try
            //{
            //    ValidateRequestHeader(Request);
            //}
            //catch (System.Web.Mvc.HttpAntiForgeryException)
            //{
            //    return null;
            //}

            //var cookies = Request.Headers.GetCookies("currency");

            //if (cookies != null && cookies.Any())
            //{
            //    var cookie = cookies.First().Cookies.SingleOrDefault(t => t.Name == "currency");

            //    if (cookie != null)
            //    {
            //        value.Currency = cookie.Value;
            //    }
            //}

            if (string.IsNullOrEmpty(value.Currency))
            {
                value.Currency = "EUR";
            }

            var response = await nexusClient.SimulateBuyBroker(new SimulateBuyBrokerRequest
            {
                AccountCode = value.AccountCode,
                CryptoAmount = value.CryptoAmount,
                Currency = value.Currency,
                PaymentMethodCode = value.PaymentMethodCode
            });

            if (response.IsSuccess)
            {
                return Ok(response.Values);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("CheckSellInfo/{id?}")]
        public async Task<SellInfoPT> CheckSellInfo(string id = null)
        {
            //try
            //{
            //    ValidateRequestHeader(Request);
            //}
            //catch (System.Web.Mvc.HttpAntiForgeryException)
            //{
            //    return null;
            //}

            SellInfoRequestPT model = new SellInfoRequestPT()
            {
                AccountCode = id,
                //IP = HttpContext.Current.Request.UserHostAddress
                IP = "::1"
            };

            SellInfoPT response = await nexusClient.PostAndGetRequestAsync<SellInfoPT, SellInfoRequestPT>("api/Sell/PostProcessInformation/", model);

            return response;
        }

        public class GetEuroValue
        {
            public string AccountCode { get; set; }
            public decimal BtcAmount { get; set; }
            public string Currency { get; set; }
        }

        [HttpPost("EuroValue")]
        public async Task<ActionResult<SimulateSellBrokerResponse>> EuroValue([FromBody]GetEuroValue value)
        {
            //try
            //{
            //    ValidateRequestHeader(Request);
            //}
            //catch (System.Web.Mvc.HttpAntiForgeryException)
            //{
            //    return null;
            //}

            //var cookies = Request.Headers.GetCookies("currency");

            //if (cookies != null && cookies.Any())
            //{
            //    var cookie = cookies.First().Cookies.SingleOrDefault(t => t.Name == "currency");

            //    if (cookie != null)
            //    {
            //        value.Currency = cookie.Value;
            //    }
            //}

            if (string.IsNullOrEmpty(value.Currency))
            {
                value.Currency = "EUR";
            }

            var response = await nexusClient.SimulateSellBroker(new SimulateBuyBrokerRequest
            {
                AccountCode = value.AccountCode,
                PaymentMethodCode = "DC_CRYPTO_BTC_EUR",
                CryptoAmount = value.BtcAmount,
                Currency = value.Currency
            });

            if (response.IsSuccess)
            {
                return Ok(response.Values);
            }
            else
            {
                return BadRequest(response.Errors);
            }
        }
    }
}