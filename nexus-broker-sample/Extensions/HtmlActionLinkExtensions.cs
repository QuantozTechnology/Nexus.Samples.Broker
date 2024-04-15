using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq.Expressions;

namespace Nexus.Samples.Broker.Extensions
{
    public static class HtmlActionLinkExtensions
    {
        public static IHtmlContent ActionLink<T, TController>(this IHtmlHelper<T> htmlHelper, string linkText, Expression<Func<TController, object>> actionExpression)
        {
            return ActionLink(htmlHelper,
                              linkText,
                              actionExpression,
                              null);
        }

        public static IHtmlContent ActionLink<T, TController>(this IHtmlHelper<T> htmlHelper, string linkText, Expression<Func<TController, object>> actionExpression, object values)
        {
            return htmlHelper.ActionLink(linkText,
                actionExpression.GetActionName(),
                typeof(TController).GetControllerName(),
                routeValues: values,
                protocol: "https",
                hostname: "",
                fragment: "",
                htmlAttributes: null);
        }

        public static IHtmlContent ActionLink<T, TController>(this IHtmlHelper<T> htmlHelper, string linkText, Expression<Func<TController, object>> actionExpression, object values, object htmlAttributes)
        {
            return htmlHelper.ActionLink(linkText,
                actionExpression.GetActionName(),
                typeof(TController).GetControllerName(),
                routeValues: values,
                protocol: "https",
                hostname: "",
                fragment: "",
                htmlAttributes: htmlAttributes);
        }

        public static string Action<TController>(this IUrlHelper urlHelper, Expression<Func<TController, object>> actionExpression)
        {
            return urlHelper.Action(actionExpression.GetActionName(), typeof(TController).GetControllerName(), null);
        }

        public static string Action<TController>(this IUrlHelper urlHelper, Expression<Func<TController, object>> actionExpression, object values)
        {
            return urlHelper.Action(actionExpression.GetActionName(), typeof(TController).GetControllerName(), values);
        }

        public static string ActionAbs<TController>(this IUrlHelper urlHelper, Expression<Func<TController, object>> actionExpression)
        {
            return urlHelper.Action(actionExpression.GetActionName(), typeof(TController).GetControllerName(), null);
        }

        public static string ActionAbs<TController>(this IUrlHelper urlHelper, Expression<Func<TController, object>> actionExpression, object values)
        {
            return urlHelper.Action(actionExpression.GetActionName(), typeof(TController).GetControllerName(), values);
        }

        /// <summary>
        /// Gets the name of the controller out of the type name, assuming this type is a controller of course....
        /// </summary>
        /// <param name="controllerType">type to extract name from.</param>
        /// <returns>name minus controller</returns>
        public static string GetControllerName(this Type controllerType)
        {
            // This is based on an MVC Convention:
            return controllerType.Name.Replace("Controller", string.Empty);
        }

        /// <summary>
        /// Extract the action name out of this expression.
        /// </summary>
        /// <param name="actionExpression"></param>
        /// <returns></returns>
        public static string GetActionName(this LambdaExpression actionExpression)
        {
            // Convention: we are expecting that the body of the expression
            // given is a Method Call.  Once we have that, we can extract
            // the Name from the expression.
            return ((MethodCallExpression)actionExpression.Body).Method.Name;
        }
    }
}