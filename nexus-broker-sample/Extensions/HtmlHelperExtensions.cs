using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Nexus.Samples.Broker.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static string GetControllerName<T>(this IHtmlHelper<T> html)
        {
            return html.ViewContext.RouteData.Values["controller"]?.ToString()?.ToLower();
        }

        public static string GetActiveCrypto<T>(this IHtmlHelper<T> html)
        {
            return html.ViewContext.RouteData.Values.ContainsKey("crypto")
                ? html.ViewContext.RouteData.Values["crypto"].ToString().ToLower()
                : "";
        }

        public static string GetActionName<T>(this IHtmlHelper<T> html)
        {
            return html.ViewContext.RouteData.Values["action"]?.ToString()?.ToLower();
        }

        public static string GetLanguage<T>(this IHtmlHelper<T> html)
        {
            var language = html.ViewContext.RouteData.Values["culture"]?.ToString();
            return language;
        }

        public static string GetLanguageFromCookie<T>(this IHtmlHelper<T> html)
        {
            return html.ViewContext.HttpContext.Request.Cookies["lang"];
        }

        public static string Obfuscate<T>(this IHtmlHelper<T> html, string str, int start = 3, int end = 3)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            if (str.Length <= (start + end + 4))
            {
                if (start > 0) start--;
                if (end > 0) end--;
                if (str.Length <= (start + end + 4))
                {
                    if (start > 0) start--;
                    if (end > 0) end--;
                    if (str.Length <= (start + end + 2))
                    {
                        return "******";
                    }
                }
            }
            string firstPart = str.Substring(0, start);
            string middlePart = new string('*', str.Length - start - end);
            string endPart = str.Substring(str.Length - end);
            return firstPart + middlePart + endPart;
        }

        public static string ObfuscateEmail<T>(this IHtmlHelper<T> html, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return string.Empty;
            }

            int nameLength = email.Substring(0, email.IndexOf('@')).Length;

            if (nameLength > 3)
            {
                string firstPart = email.Substring(0, 2);
                string middlePart = new string(email.Substring(2, email.IndexOf('@') - 2).Select(x => '*').ToArray());
                string endPart = email.Substring(email.IndexOf('@'));
                return firstPart + middlePart + endPart;
            }
            else
            {
                return "***" + email.Substring(email.IndexOf('@'));
            }
        }

        public static string GetWrapperClass<T>(this IHtmlHelper<T> html)
        {
            string controllerName = html.GetControllerName();
            string actionName = html.GetActionName();

            if ((controllerName == "account") && (actionName == "index") && (html.ViewBag.Created == false))
            {
                return controllerName;
            }

            return string.Empty;
        }

        public static string GetPageDescription<T>(this IHtmlHelper<T> html)
        {
            string controllerName = html.GetControllerName()?.ToLower();
            string actionName = html.GetActionName()?.ToLower();

            string description = Resources.Site.HomePageDescription;

            if (controllerName == "buy")
            {
                description = Resources.Site.BuyPageDescription;
            }
            else if (controllerName == "sell")
            {
                description = Resources.Site.SellPageDescription;
            }
            else if (controllerName == "account")
            {
                if (actionName == "createadditional")
                {
                    description = Resources.Site.AditionalAccountPageDescription;
                }
                else if (actionName == "requestinfo")
                {
                    description = Resources.Site.RequestInfoPageDescription;
                }
                else if (actionName == "requestdelete")
                {
                    description = Resources.Site.RequestDeletePageDescription;
                }
                else
                {
                    description = Resources.Site.AccountPageDescription;
                }
            }

            return description;
        }
    }
}