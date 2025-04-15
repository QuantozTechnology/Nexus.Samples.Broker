using System;
using System.Collections.Generic;

namespace Nexus.Samples.Sdk.Models.Response
{
    public class GetPricesResponse
    {
        public DateTime Created { get; set; }
        public string CurrencyCode { get; set; }
        public IDictionary<string, GetPricesResponseItem> Prices { get; set; }

    }

    public class GetPricesResponseItem
    {
        public decimal? Buy { get; set; }
        public decimal? Sell { get; set; }
        public decimal? EstimatedNetworkSlowFee { get; set; }
        public decimal? EstimatedNetworkFastFee { get; set; }
        public DateTime? Updated { get; set; }
    }
}
