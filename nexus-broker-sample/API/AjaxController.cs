﻿using Microsoft.AspNetCore.Mvc;
using Nexus.Samples.Broker.Configuration;
using Nexus.Samples.Broker.Extensions;
using Nexus.Samples.Broker.ViewModels;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models;
using Nexus.Samples.Sdk.Models.Request;
using Nexus.Samples.Sdk.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexus.Samples.Broker.API
{
    [Route("api/ajax")]
    [ApiController]
    public class AjaxController : ControllerBase
    {
        private readonly NexusClient _nexusClient;
        private readonly SupportedCryptoHelper _supportedCryptoHelper;
        private const string CurrencyCode = "EUR";

        public AjaxController(NexusClient nexusClient, SupportedCryptoHelper supportedCryptoHelper)
        {
            this._nexusClient = nexusClient;
            this._supportedCryptoHelper = supportedCryptoHelper;
        }

        [HttpGet("getprices")]
        public async Task<ActionResult<MarketPricesModel>> GetPrices([FromQuery] string currency)
        {
            var getPricesResponse = await _nexusClient.GetPrices(currency);

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
            var accountResponse = await this._nexusClient.GetAccount(id);
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

            var paymentMethodResponse = await this._nexusClient.GetPaymentMethodInformation(CurrencyCode, account.DcCode, "BUY");
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

            var transactionsAwaitingPayment = await this._nexusClient.GetTransactions(account.CustomerCode, "BuyIncasso");
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
                PaymentMethods = availablePaymentMethods,
                PaymentPending = paymentPending,
                Limits = limits
            };

            return Ok(info);
        }

        [HttpGet("checkbuylimits")]
        public async Task<IActionResult> CheckBuyLimits([FromQuery] string accountId, [FromQuery] string paymentMethodCode)
        {
            var accountResponse = await this._nexusClient.GetAccount(accountId);
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
            var brokerLimitResponse = await this._nexusClient.GetBrokerBuyLimits(customerCode, paymentMethodCode);
            if (!brokerLimitResponse.IsSuccess)
            {
                return (null, brokerLimitResponse.Errors);
            }
            var brokerBuyLimits = brokerLimitResponse.Values;
            var info = new LimitInfoModel()
            {
                LimitReasons = brokerBuyLimits.LimitReasons,
                RemainingDailyLimit = brokerBuyLimits.Remaining.DailyLimit,
                MinimumAmount = 0, //TODO: Replace with API result after fixed
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
        public async Task<ActionResult<SimulateBuyBrokerResponse>> SimulateBuy([FromBody] SimulateBuyBrokerRequest value)
        {
            if (string.IsNullOrEmpty(value.Currency))
            {
                value.Currency = CurrencyCode;
            }

            var response = await _nexusClient.SimulateBuyBroker(value);

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

        [HttpGet("checksellinfo/{id?}")]
        public async Task<IActionResult> CheckSellInfo(string id = null)
        {
            var accountResponse = await this._nexusClient.GetAccount(id);
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

            var brokerLimitResponse = await this._nexusClient.GetBrokerSellLimits(account.CustomerCode, account.DcCode);
            if (!brokerLimitResponse.IsSuccess)
            {
                return BadRequest(brokerLimitResponse.Errors);
            }
            var brokerSellLimits = brokerLimitResponse.Values;

            var pricesResponse = await this._nexusClient.GetPrices("EUR");
            if (!pricesResponse.IsSuccess)
            {
                return BadRequest(pricesResponse.Errors);
            }
            var prices = pricesResponse.Values;
            var cryptoPrices = prices.Prices[account.DcCode];

            var brokerBuyTransactionsResponse = await this._nexusClient.GetBuyTransactions(account.CustomerCode);
            if (!brokerBuyTransactionsResponse.IsSuccess)
            {
                return BadRequest(brokerBuyTransactionsResponse?.Errors);
            }
            var brokerBuyTransactions = brokerBuyTransactionsResponse.Values;

            SellInfoPT response = new SellInfoPT()
            {
                AccountValid = account.AccountStatus.ToUpper() == "ACTIVE",
                TrustLevel = customer.Trustlevel.ToUpper(),
                Currency = CurrencyCode,
                DCCode = account.DcCode,
                IsBusiness = customer.IsBusiness,
                MinBtcAmount = brokerSellLimits.Remaining.DailyLimit / 2,//TODO: If new API available rather call API
                MaxBtcAmount = brokerSellLimits.Remaining.DailyLimit,
                SellServiceAvailable = cryptoPrices != null && cryptoPrices.Sell > 0 && ((DateTime.UtcNow - cryptoPrices.Updated.GetValueOrDefault(DateTime.UtcNow)).TotalMinutes < 30),
            };

            return Ok(response);
        }

        public class SimulateSell
        {
            public string AccountCode { get; set; }
            public decimal BtcAmount { get; set; }
            public string Currency { get; set; }
            public string CryptoCode { get; set; }
        }

        [HttpPost("simulatesell")]
        public async Task<ActionResult<SimulateSellBrokerResponse>> SimulateBrokerSell([FromBody] SimulateSell value)
        {
            if (string.IsNullOrEmpty(value.Currency))
            {
                value.Currency = CurrencyCode;
            }

            var paymentMethodCode = _supportedCryptoHelper.GetSupportedCrypto(value.CryptoCode).SellPaymentMethodCode;

            var response = await _nexusClient.SimulateSellBroker(new SimulateSellBrokerRequest
            {
                AccountCode = value.AccountCode,
                PaymentMethodCode = paymentMethodCode,
                CryptoAmount = value.BtcAmount,
                Ip = Request.HttpContext.Connection.RemoteIpAddress.ToString()
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

        [HttpGet("getsupportedcrypto/{crypto}")]
        public async Task<ActionResult<SupportedCrypto>> GetSupportedCrypto(string crypto)
        {
            return Ok(_supportedCryptoHelper.GetSupportedCrypto(crypto));
        }
    }
}