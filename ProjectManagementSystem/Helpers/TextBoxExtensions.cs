using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ProjectManagementSystem.Helpers
{
    public static class TextBoxExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString TextBox<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return InputExtensions.TextBoxFor(htmlHelper, expression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString TextBox<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return InputExtensions.TextBoxFor(htmlHelper, expression, htmlAttributes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static MvcHtmlString TextBox<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format)
        {
            return InputExtensions.TextBoxFor(htmlHelper, expression, format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="format"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString TextBox<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format, object htmlAttributes)
        {
            return InputExtensions.TextBoxFor(htmlHelper, expression, format, htmlAttributes);
        }
        //////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id">ID</param>
        /// <param name="colSize"></param>
        /// <returns></returns>
        public static MvcHtmlString TextBox(this HtmlHelper helper, string id, string colSize)
        {
            return MvcHtmlString.Create(
                String.Format("<input type='text' for='{0}' class='col_{1} left'></input>", id, colSize));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <param name="colSize"></param>
        /// <returns></returns>
        public static MvcHtmlString TextBox(this HtmlHelper helper, string id, string text, int colSize)
        {
            return MvcHtmlString.Create(
                String.Format("<input type='text' for='{0}' class='col_{2} left' value='{1}'></input>", id, text, colSize));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString TextBoxColorPicker<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            htmlAttributes = new {@class = "color" };
            return InputExtensions.TextBoxFor(htmlHelper, expression, htmlAttributes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="format"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString TextBoxDateTime<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper, 
            Expression<Func<TModel, TProperty>> expression, 
            string format, 
            object htmlAttributes)
        {
            htmlAttributes = new { type = "date", @class = "datefield" };

            if (string.IsNullOrEmpty(format))
            {
                format = "{0:d}";
            }

            return InputExtensions.TextBoxFor(htmlHelper, expression, format, htmlAttributes);
        }
    }
}