using System;
using System.Text;
using Microsoft.AspNetCore.Html;

namespace Raydreams.Web.Extensions
{
    public static class HTMLExtensions
    {
        /// <summary>
        /// Injects a Google Analytics tag into the page
        /// </summary>
        /// <param name="code">Your ananlytics code</param>
        /// <returns></returns>
        public static HtmlString GoogleAnalytics(string code)
        {
            if (String.IsNullOrWhiteSpace(code))
                new HtmlString("<!-- no google analytics code -->");

            code = code.Trim();

            StringBuilder sb = new StringBuilder($"<script async src=\"https://www.googletagmanager.com/gtag/js?id={code}\"></script>");
            sb.Append(Environment.NewLine);
            sb.Append($"<script type=\"text/javascript\">window.dataLayer = window.dataLayer || []; function gtag() {{ dataLayer.push(arguments); }} gtag('js', new Date()); gtag('config', '{code}');</script>");

            return new HtmlString(sb.ToString());
        }
    }
}
