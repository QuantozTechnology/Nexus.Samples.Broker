using Microsoft.AspNetCore.Mvc;
using Nexus.Samples.Broker.ViewModels;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models;
using Nexus.Samples.Sdk.Models.Request;
using Nexus.Samples.Sdk.Models.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexus.Samples.Broker.API
{
    [Route("api/ajax")]
    [ApiController]
    public class AjaxController : ControllerBase
    {
        private readonly NexusClient nexusClient;
        private const string CurrencyCode = "EUR";

        public AjaxController(NexusClient nexusClient)
        {
            this.nexusClient = nexusClient;
        }

        [HttpGet("getprices")]
        public async Task<ActionResult<MarketPricesModel>> GetPrices([FromQuery]string currency)
        {
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
            var accountResponse = await this.nexusClient.GetAccount(id);
            if (!accountResponse.IsSuccess)
            {
                return BadRequest(accountResponse.Errors);
            }
            var account = accountResponse.Values;

            var customerResponse = await this.nexusClient.GetCustomer(account.CustomerCode);
            if (!customerResponse.IsSuccess)
            {
                return BadRequest(customerResponse.Errors);
            }
            var customer = customerResponse.Values;

            var paymentMethodResponse = await this.nexusClient.GetPaymentMethodInformation(CurrencyCode, account.DcCode, "BUY");
            if (!paymentMethodResponse.IsSuccess)
            {
                return BadRequest(paymentMethodResponse.Errors);
            }

            PaymentMethod[] availablePaymentMethods = null;
            if (paymentMethodResponse.Values.Total > 0)
            {
                var paymentMethods = paymentMethodResponse.Values.Records;
                var uniquePMs = paymentMethods.DistinctBy(pm => pm.PaymentType.Code);
                availablePaymentMethods = uniquePMs.Select(pm => new PaymentMethod()
                {
                    PaymentMethodCode = pm.Code,
                    PaymentMethodName = pm.Name,
                    PaymentTypeCode = pm.PaymentType.Code,
                    PaymentTypeDisplay = string.IsNullOrEmpty(pm.PaymentType.Description) ? pm.PaymentType.Name : pm.PaymentType.Description,
                }).ToArray();
            }          

            var transactionsAwaitingPayment = await this.nexusClient.GetTransactions(account.CustomerCode, "BuyIncasso");
            var paymentPending = (transactionsAwaitingPayment.IsSuccess && transactionsAwaitingPayment.Values.Total > 0);

            var (limits, limitErrors) = await RetrieveLimitInfoModel(account.CustomerCode, availablePaymentMethods.First().PaymentMethodCode);
            if (limitErrors != null)
            {
                return BadRequest(limitErrors);
            }

            var info = new BuyInfoModel()
            {
                AccountValid = account.AccountStatus.ToUpper() == "ACTIVE",
                DCCode = account.DcCode,
                Currency = CurrencyCode,
                CoolingDown = false,
                FirstBuyStatus = string.IsNullOrEmpty(customer.FirstTransaction) ? 1 : 0,
                NeedPhotoID = !customer.HasPhotoId,
                PaymentMethods = availablePaymentMethods,
                PaymentPending = paymentPending,
                Limits = limits
            };

            return Ok(info);            
        }

        [HttpGet("checkbuylimits")]
        public async Task<IActionResult> CheckBuyLimits([FromQuery] string accountId, [FromQuery] string paymentMethodCode)
        {
            var accountResponse = await this.nexusClient.GetAccount(accountId);
            if (!accountResponse.IsSuccess)
            {
                return BadRequest(accountResponse.Errors);
            }
            var account = accountResponse.Values;

            var (info, errors) = await RetrieveLimitInfoModel(account.CustomerCode, paymentMethodCode);
            if (errors != null)
            {
                return BadRequest(errors);
            }

            return Ok(info);
        }

        private async Task<(LimitInfoModel, IEnumerable<string>)> RetrieveLimitInfoModel(string customerCode, string paymentMethodCode)
        {
            var brokerLimitResponse = await this.nexusClient.GetBrokerBuyLimit(customerCode, paymentMethodCode);
            if (!brokerLimitResponse.IsSuccess)
            {
                return (null, brokerLimitResponse.Errors);
            }
            var brokerBuyLimits = brokerLimitResponse.Values;
            var info = new LimitInfoModel()
            {
                LimitReasons = brokerBuyLimits.LimitReasons,
                RemainingDailyLimit = brokerBuyLimits.Remaining.DailyLimit,
                MinimumAmount = brokerBuyLimits.Remaining.DailyLimit / 2, //TODO: Replace with API result after fixed
            };

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

            return (info, null);
        }

        [HttpPost("simulatebuy")]
        public async Task<ActionResult<SimulateBuyBrokerResponse>> SimulateBuy([FromBody]SimulateBuyBrokerRequest value)
        {
            if (string.IsNullOrEmpty(value.Currency))
            {
                value.Currency = CurrencyCode;
            }

            var response = await nexusClient.SimulateBuyBroker(value);

            if (response.IsSuccess)
            {
                return Ok(response.Values);
            }
            else
            {
                if (response.Values != null)
                {
                    return BadRequest(response.Values);
                } 
                else
                {
                    return BadRequest();
                }
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
            public string CryptoCode { get; set; }
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
                value.Currency = CurrencyCode;
            }

            var response = await nexusClient.SimulateSellBroker(new SimulateSellBrokerRequest
            {
                AccountCode = value.AccountCode,
                PaymentMethodCode = $"DC_CRYPTO_{value.CryptoCode}_EUR",
                CryptoAmount = value.BtcAmount,
                Ip = "::1"
                //Currency = value.Currency
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