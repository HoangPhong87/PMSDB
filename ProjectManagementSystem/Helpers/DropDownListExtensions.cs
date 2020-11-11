using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace ProjectManagementSystem.Helpers
{
    public static class DropDownListExtensions
    {
        
       
        /// <summary>
        /// Return the Html's select element
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="addEmptyRow"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            object htmlAttributes,
            bool addEmptyRow)
        {
            var list = new List<SelectListItem>();

            if (addEmptyRow)
            {
                
                list.Add(new SelectListItem()
                {
                    Value = string.Empty,
                    Text = "選択してください"
                });
            }

            if (selectList != null)
            {
                list.AddRange(selectList);
            }

            return SelectExtensions.DropDownListFor(
                htmlHelper,
                expression,
                list,
                htmlAttributes
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="addEmptyRow"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownListChildFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            object htmlAttributes,
            bool addEmptyRow)
        {
            var list = new List<SelectListItem>();

            if (addEmptyRow)
            {
                
                list.Add(new SelectListItem()
                {
                    Value = string.Empty,
                    Text = string.Empty
                });
            }

            if (selectList != null)
            {
                list.AddRange(selectList);
            }

            
            var resultAttr = new RouteValueDictionary();

            
            resultAttr.Add("data-bind", "options: childDropDownList, optionsText: 'Text', optionsValue: 'Value', value: childDropDownListSelectedValue, optionsCaption: '選択してください', enable: childDropDownList().length !== 0");

            
            var preAttrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            
            if (!preAttrs.ContainsKey("class"))
            {
                resultAttr.Add("class", "datefield");
            }
            else
            {
                resultAttr.Add("class", "datefield " + preAttrs["class"]);
            }

            return SelectExtensions.DropDownListFor(
                htmlHelper,
                expression,
                list,
                resultAttr
            );
        }
    }
}