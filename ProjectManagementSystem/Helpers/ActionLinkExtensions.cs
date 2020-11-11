using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace ProjectManagementSystem.Helpers
{
    public static class ActionLinkExtensions
    {
        /// <summary>
        /// 指定されたアクションの仮想パスを格納しているアンカー要素 (a 要素) を返します。
        /// </summary>
        /// <param name="htmlHelper">このメソッドによって拡張される HTML ヘルパー インスタンス。</param>
        /// <param name="functionCode">機能コード</param>
        /// <param name="linkText">アンカー要素の内部テキスト。</param>
        /// <param name="actionName">アクションの名前。</param>
        /// <returns>アンカー要素 (a 要素)。</returns>
        public static MvcHtmlString AuthCheckLink(
            this HtmlHelper htmlHelper,
            string functionCode,
            string linkText,
            string actionName
        )
        {
            if (HasAuth(htmlHelper, functionCode))
            {
                return htmlHelper.ActionLink(linkText, actionName);
            }
            else
            {
                return MvcHtmlString.Empty;
            }
        }

        /// <summary>
        /// 指定されたアクションの仮想パスを格納しているアンカー要素 (a 要素) を返します。
        /// </summary>
        /// <param name="htmlHelper">このメソッドによって拡張される HTML ヘルパー インスタンス。</param>
        /// <param name="functionCode">機能コード</param>
        /// <param name="linkText">アンカー要素の内部テキスト。</param>
        /// <param name="actionName">アクションの名前。</param>
        /// <param name="controllerName">コントローラーの名前。</param>
        /// <returns>アンカー要素 (a 要素)。</returns>
        public static MvcHtmlString AuthCheckLink(
            this HtmlHelper htmlHelper,
            string functionCode,
            string linkText,
            string actionName,
            string controllerName
        )
        {
            if (HasAuth(htmlHelper, functionCode))
            {
                return htmlHelper.ActionLink(linkText, actionName, controllerName);
            }
            else
            {
                return MvcHtmlString.Empty;
            }
        }

        /// <summary>
        /// 指定されたアクションの仮想パスを格納しているアンカー要素 (a 要素) を返します。
        /// </summary>
        /// <param name="htmlHelper">このメソッドによって拡張される HTML ヘルパー インスタンス。</param>
        /// <param name="functionCode">機能コード</param>
        /// <param name="linkText">アンカー要素の内部テキスト。</param>
        /// <param name="actionName">アクションの名前。</param>
        /// <param name="routeValues">ルートのパラメーターが含まれるオブジェクト。 オブジェクトのプロパティを調べることで、リフレクションを介してパラメーターが取得されます。 通常、このオブジェクトは、オブジェクト初期化子構文を使用して作成されます。 </param>
        /// <returns>アンカー要素 (a 要素)。</returns>
        public static MvcHtmlString AuthCheckLink(
            this HtmlHelper htmlHelper,
            string functionCode,
            string linkText,
            string actionName,
            Object routeValues
        )
        {
            if (HasAuth(htmlHelper, functionCode))
            {
                return htmlHelper.ActionLink(linkText, actionName, routeValues);
            }
            else
            {
                return MvcHtmlString.Empty;
            }
        }

        /// <summary>
        /// 指定されたアクションの仮想パスを格納しているアンカー要素 (a 要素) を返します。
        /// </summary>
        /// <param name="htmlHelper">このメソッドによって拡張される HTML ヘルパー インスタンス。</param>
        /// <param name="functionCode">機能コード</param>
        /// <param name="linkText">アンカー要素の内部テキスト。</param>
        /// <param name="actionName">アクションの名前。</param>
        /// <param name="routeValues">ルートのパラメーターが含まれるオブジェクト。 オブジェクトのプロパティを調べることで、リフレクションを介してパラメーターが取得されます。 通常、このオブジェクトは、オブジェクト初期化子構文を使用して作成されます。 </param>
        /// <param name="htmlAttributes">この要素の HTML 属性を格納するオブジェクト。 オブジェクトのプロパティを調べることで、リフレクションを介して属性が取得されます。 通常、このオブジェクトは、オブジェクト初期化子構文を使用して作成されます。 </param>
        /// <returns>アンカー要素 (a 要素)。</returns>
        public static MvcHtmlString AuthCheckLink(
            this HtmlHelper htmlHelper,
            string functionCode,
            string linkText,
            string actionName,
            Object routeValues,
            Object htmlAttributes
        )
        {
            if (HasAuth(htmlHelper, functionCode))
            {
                return htmlHelper.ActionLink(linkText, actionName, routeValues, htmlAttributes);
            }
            else
            {
                return MvcHtmlString.Empty;
            }
        }

        /// <summary>
        /// 指定されたアクションの仮想パスを格納しているアンカー要素 (a 要素) を返します。
        /// </summary>
        /// <param name="htmlHelper">このメソッドによって拡張される HTML ヘルパー インスタンス。</param>
        /// <param name="functionCode">機能コード</param>
        /// <param name="linkText">アンカー要素の内部テキスト。</param>
        /// <param name="actionName">アクションの名前。</param>
        /// <param name="controllerName">コントローラーの名前。</param>
        /// <param name="routeValues">ルートのパラメーターが含まれるオブジェクト。 オブジェクトのプロパティを調べることで、リフレクションを介してパラメーターが取得されます。 通常、このオブジェクトは、オブジェクト初期化子構文を使用して作成されます。 </param>
        /// <param name="htmlAttributes">この要素の HTML 属性を格納するオブジェクト。 オブジェクトのプロパティを調べることで、リフレクションを介して属性が取得されます。 通常、このオブジェクトは、オブジェクト初期化子構文を使用して作成されます。 </param>
        /// <returns>アンカー要素 (a 要素)。</returns>
        public static MvcHtmlString AuthCheckLink(
            this HtmlHelper htmlHelper,
            string functionCode,
            string linkText,
            string actionName,
            string controllerName,
            Object routeValues,
            Object htmlAttributes
        )
        {
            if (HasAuth(htmlHelper, functionCode))
            {
                return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
            }
            else
            {
                return MvcHtmlString.Empty;
            }
        }

        /// <summary>
        /// ログインユーザに指定された権限があるかどうかを返します。
        /// </summary>
        /// <param name="htmlHelper">HTML ヘルパー インスタンス。</param>
        /// <param name="functionCode">機能コード</param>
        /// <returns>権限がある場合のみ true を返す。</returns>
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

        /// <summary>
        /// 新規登録ボタンを生成します。
        /// </summary>
        /// <param name="helper">このメソッドによって拡張される HTML ヘルパー インスタンス。</param>
        /// <param name="hyperLinkID">ID</param>
        /// <param name="href">リンク</param>
        /// <param name="linkText">アンカー要素の内部テキスト。</param>
        /// <returns>アンカー要素 (a 要素)。</returns>
        public static MvcHtmlString ActionLink_Plus(this HtmlHelper helper, string hyperLinkID, string href, string linkText)
        {
            return MvcHtmlString.Create(
                String.Format("<a id='{0}' name='{0}' href='{1}' class='button blue icon-plus'>{2}</a>"
                    , hyperLinkID
                    , href
                    , linkText
                    ));
        }

    }
}