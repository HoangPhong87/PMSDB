using System.Web.Mvc;
using System.Web.Routing;

namespace ProjectManagementSystem.Helpers
{
    public static class ButtonExtensions
    {
        /// <summary>
        /// Return the input type button
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString Button(
            this HtmlHelper htmlHelper,
            string id,
            string value,
            object htmlAttributes)
        {
            var builder = new TagBuilder("button");
            builder.Attributes.Add("id", id);
            builder.Attributes.Add("name", id);
            builder.Attributes.Add("value", value);
            builder.InnerHtml = value;

            var htmlAttr = new RouteValueDictionary(htmlAttributes);

            if (!htmlAttr.ContainsKey("type"))
            {
                
                builder.MergeAttribute("type", "button");
            }

            builder.MergeAttributes((new RouteValueDictionary(htmlAttr)));

            return MvcHtmlString.Create(builder.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static MvcHtmlString AuthCheckButton(
            this HtmlHelper htmlHelper,
            string id,
            string value,
            object htmlAttributes,
            object routeValues)
        {
            var builder = new TagBuilder("button");
            builder.Attributes.Add("id", id);
            builder.Attributes.Add("name", id);
            builder.Attributes.Add("value", value);
            builder.InnerHtml = value;

            var htmlAttr = new RouteValueDictionary(htmlAttributes);

            if (!htmlAttr.ContainsKey("type"))
            {
                
                builder.MergeAttribute("type", "button");
            }

            builder.MergeAttributes((new RouteValueDictionary(htmlAttr)));
            builder.MergeAttributes((new RouteValueDictionary(routeValues)));

            return MvcHtmlString.Create(builder.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="functionCode"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString AuthCheckButton(
            this HtmlHelper htmlHelper,
            string functionCode,
            string id,
            string value,
            object htmlAttributes)
        {
            var builder = new TagBuilder("button");
            builder.Attributes.Add("id", id);
            builder.Attributes.Add("name", id);
            builder.Attributes.Add("value", value);
            builder.InnerHtml = value;

            var htmlAttr = new RouteValueDictionary(htmlAttributes);

            if (!htmlAttr.ContainsKey("type"))
            {
                
                builder.MergeAttribute("type", "button");
            }

            if (!HasAuth(htmlHelper, functionCode))
            {
                
                if (!htmlAttr.ContainsKey("disabled"))
                {
                    builder.MergeAttribute("disabled", "disabled");
                }
            }

            builder.MergeAttributes((new RouteValueDictionary(htmlAttr)));

            return MvcHtmlString.Create(builder.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="functionCode"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static MvcHtmlString AuthCheckButton(
            this HtmlHelper htmlHelper,
            string functionCode,
            string id,
            string value,
            object htmlAttributes,
            object routeValues)
        {
            var builder = new TagBuilder("button");
            builder.Attributes.Add("id", id);
            builder.Attributes.Add("name", id);
            builder.Attributes.Add("value", value);
            builder.InnerHtml = value;

            var htmlAttr = new RouteValueDictionary(htmlAttributes);

            if (!htmlAttr.ContainsKey("type"))
            {
                
                builder.MergeAttribute("type", "button");
            }

            if (!HasAuth(htmlHelper, functionCode))
            {
                if (!htmlAttr.ContainsKey("disabled"))
                {
                    builder.MergeAttribute("disabled", "disabled");
                }
            }

            builder.MergeAttributes((new RouteValueDictionary(htmlAttr)));
            builder.MergeAttributes((new RouteValueDictionary(routeValues)));

            return MvcHtmlString.Create(builder.ToString());
        }

        /// <summary>
        /// Check for authentication status
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        private static bool HasAuth(HtmlHelper htmlHelper, string functionCode)
        {
            if (!string.IsNullOrEmpty(functionCode))
            {
                var user = htmlHelper.ViewBag.LoginUser;

                return user.GetAuth(functionCode);
            }
            else
            {
                return true;
            }
        }
    }
}