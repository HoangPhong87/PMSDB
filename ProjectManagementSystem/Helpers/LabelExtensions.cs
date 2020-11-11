using System;
using System.Web.Mvc;

namespace ProjectManagementSystem.Helpers
{
    public static class LabelExtensions
    {
       /// <summary>
       /// 
       /// </summary>
       /// <param name="helper"></param>
       /// <param name="id"></param>
       /// <param name="text"></param>
       /// <returns></returns>
        public static MvcHtmlString Label(this HtmlHelper helper, string id, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return MvcHtmlString.Create(String.Format("<label for='{0}'>{1}</label>", id, text));
            }
            else
            {
                
                return MvcHtmlString.Create("<label></label>");
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <param name="colSize"></param>
        /// <returns></returns>
        public static MvcHtmlString Label(this HtmlHelper helper, string id, string text, int colSize)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return MvcHtmlString.Create(String.Format("<label for='{0}'>{1}</label>", id, text, colSize));
            }
            else
            {
                
                return MvcHtmlString.Create("<label></label>");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <param name="colSize"></param>
        /// <returns></returns>
        public static MvcHtmlString LabelTitle(this HtmlHelper helper, string id, object text, int colSize)
        {
            if (text != null)
            {
                return MvcHtmlString.Create(String.Format("<label for='{0}' class='col_{2}'>{1}</label>", id, text, colSize));
            }
            else
            {
                
                return MvcHtmlString.Create("<label></label>");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="text"></param>
        /// <param name="colSize"></param>
        /// <returns></returns>
        public static MvcHtmlString LabelLoginUserInfo(this HtmlHelper helper, object text, int colSize)
        {
            if (text != null)
            {
                return MvcHtmlString.Create(String.Format("<label class='col_{1} login_user_info'>{0}</label>", text, colSize));
            }
            else
            {
                return MvcHtmlString.Create("<label></label>");
            }
        }
    }
}