using Nexus.Samples.Sdk.Models.Response;
using System.Collections.Generic;
using System.Linq;

namespace Nexus.Samples.Broker.API
{
    public class MarketPricesModelItem
    {
        public string DC { get; set; }
        public string BuyText { get; set; }
        public string SellText { get; set; }

        public static MarketPricesModelItem CreateFromPT(GetPricesResponseItem pt, string dc, string currency)
        {
            return new MarketPricesModelItem
            {
                DC = dc,
                BuyText = GetMarketText(pt.Buy, dc, currency),
                SellText = GetMarketText(pt.Sell, dc, currency)
            };
        }

        private static string GetMarketText(decimal? value, string dc, string currencyText)
        {
            string dcText = "unknown";

            switch (dc)
            {
                case "ETH":
                    dcText = "ether";
                    break;
                case "BTC":
                    dcText = "bitcoin";
                    break;
                case "BCH":
                    dcText = "bcash";
                    break;
                case "LTC":
                    dcText = "litecoin";
                    break;
                case "XLM":
                    dcText = "lumen";
                    break;
                case "USDT":
                    dcText = "usdt";
                    break;
                case "USDT-ERC20":
                    dcText = "usdt";
                    break;
            }

            // always use decimal point and two digits, independent of regional settings => 0.00
            return value.HasValue ? value.Value.ToString("0.00") + "  " + currencyText + "/" + dcText : "Unavailable";
        }
    }

    public class MarketPricesModel
    {
        public IEnumerable<MarketPricesModelItem> Prices { get; set; }

        public static MarketPricesModel CreateFromPT(GetPricesResponse ptModel)
        {
            string currencyText;

            if (ptModel.CurrencyCode == "EUR")
            {
                currencyText = "EUR";
            }
            else if (ptModel.CurrencyCode == "GBP")
            {
                currencyText = "GBP";
            }
            else if (ptModel.CurrencyCode == "CHF")
            {
                currencyText = "CHF";
            }
            else if (ptModel.CurrencyCode == "CAD")
            {
                currencyText = "CAD";
            }
            else
            {
                currencyText = "EUR";
            }

            return new MarketPricesModel()
            {
                Prices = ptModel.Prices.Select(p => MarketPricesModelItem.CreateFromPT(p.Value, p.Key, currencyText))
            };
        }
    }
}
