#region License
/// <copyright file="PMS09004Controller.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/07/15</createdDate>
#endregion

namespace ProjectManagementSystem.Controllers
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS09004;
    using ProjectManagementSystem.ViewModels;
    using ProjectManagementSystem.ViewModels.PMS09004;
    using ProjectManagementSystem.WorkerServices;
    using ProjectManagementSystem.WorkerServices.Impl;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Work with payment sales list
    /// </summary>
    public class PMS09004Controller : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// Common service
        /// </summary>
        private readonly IPMSCommonService commonService;

        /// <summary>
        /// Main service
        /// </summary>
        private readonly IPMS09004Service mainService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS09004Controller(IPMS09004Service service, IPMSCommonService commonservice)
        {
            this.mainService = service;
            this.commonService = commonservice;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS09004Controller()
            : this(new PMS09004Service(), new PMSCommonService())
        {
        }

        #endregion

        #region Action
        /// <summary>
        /// Clear save condition
        /// </summary>
        /// <returns>Index</returns>
        [HttpGet]
        public ActionResult ClearSaveCondition()
        {
            base.ClearRestoreData();
            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>List View</returns>
        public ActionResult Index()
        {
            if (!this.IsInFunctionList(Constant.FunctionID.SalesPayment)
                && !this.IsInFunctionList(Constant.FunctionID.SalesPayment_Admin))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            string companyCode = GetLoginUser().CompanyCode;
            var model = new PMS09004ListViewModel
            {
                GROUP_LIST = this.commonService.GetUserGroupSelectList(companyCode),
                CONTRACT_TYPE_LIST = this.commonService.GetContractTypeSelectList(companyCode),
                BRANCH_LIST = this.commonService.GetBranchSelectList(companyCode)
            };

            var tmpCondition = GetRestoreData() as Condition;

            if (tmpCondition != null)
                model.Condition = tmpCondition;

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return this.View("Index", model);
        }

        /// <summary>
        /// Export CSV file
        /// </summary>
        /// <param name="condition">search condition</param>
        /// <param name="TAB_ID">id of browser tab</param>
        /// <returns>CSV file</returns>
        //[HttpPost]
        public ActionResult ExportCsv(Condition condition, string TAB_ID)
        {
            List<string> titles = new List<string>()
            {
                "年月",
                "発注元",
                "タグ名",
                "エンドユーザー名",
                "プロジェクトNo.",
                "プロジェクト名",
                "納品日",
                "検収日",
                "契約種別",
                "受注金額",
                "プロジェクト管理責任者",
                "所属",
                "ユーザ名",
                "売上金額",
                "原価",
                "予定工数（時間）",
                "予定*原価",
                "実績工数（時間）",
                "実績*原価",
                "発注先",
                "支払金額",
                "ステータス",
                "サブカテゴリ"
            };

            condition.COMPANY_CODE = GetLoginUser().CompanyCode;

            IList<PaymentSalesDetail> dataList = new List<PaymentSalesDetail>();
            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                dataList = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<PaymentSalesDetail>;
            }
            else
            {
                dataList = this.mainService.GetSalesPaymentDetailListExport(condition);
            }

            IList<PaymentSalesDetailExportCSV> dataExport = new List<PaymentSalesDetailExportCSV>();

            foreach (var data in dataList)
            {
                dataExport.Add(new PaymentSalesDetailExportCSV()
                {
                    target_time = data.target_year.ToString() + data.target_month.ToString("00"),
                    sales_company = data.sales_company,
                    tag_name = data.tag_name,
                    end_user_name = data.end_user_name,
                    project_no = data.project_no,
                    project_name = data.project_name,
                    end_date = data.end_date.HasValue ? data.end_date.Value.ToString("yyyy/MM/dd"):string.Empty,
                    acceptance_date = data.acceptance_date.HasValue ? data.acceptance_date.Value.ToString("yyyy/MM/dd"):string.Empty,
                    contract_type = data.contract_type,
                    total_sales = data.total_sales.ToString("#,##0"),
                    group_name = data.group_name,
                    user_name = data.user_name,
                    sales_amount = data.sales_amount.ToString("#,##0"),
                    charge_person = data.charge_person,
                    unit_cost = this.CheckValue(data.unit_cost),
                    plan_man_times = this.CheckWorkTime(data.plan_man_times),
                    plan_cost = this.CheckValue(data.plan_cost),
                    actual_work_time = this.CheckWorkTime(data.actual_work_time),
                    actual_cost = this.CheckValue(data.actual_cost),
                    payment_company = data.payment_company,
                    amount = this.CheckValue(data.amount),
                    status = (data.sales_type == "0") ? "売上確定分" : "見込み分",
                    sub_category = data.sub_category
                });
            }

            string fileName = "SalesPaymentList_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";
            DataTable dt = Utility.ToDataTableT(dataExport, titles.ToArray());
            Utility.ExportToCsvData(this, dt, fileName);

            return new EmptyResult();
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="COMPANY_CODE"></param>
        /// <param name="TARGET_TIME"></param>
        /// <param name="GROUP_ID"></param>
        /// <param name="USER_NAME"></param>
        /// <param name="CUSTOMER_ID"></param>
        /// <param name="DELETE_FLG"></param>
        /// <param name="PLAN_DISPLAY"></param>
        /// <param name="CONTRACT_TYPE_ID"></param>
        /// <param name="LOCATION_ID"></param>
        /// <param name="PLANNED_MEMBER_INCLUDE"></param>
        /// <param name="ESTIMATE_DISPLAY"></param>
        /// <param name="TAB_ID"></param>
        /// <returns>Json list information</returns>
        public ActionResult Search(DataTablesModel model, string COMPANY_CODE = "", string TARGET_TIME = "", string TARGET_TIME_START = "", string TARGET_TIME_END = "", string GROUP_ID = "", string USER_NAME = "", string CUSTOMER_ID = "", bool DELETE_FLG = false, bool PLAN_DISPLAY = false, string CONTRACT_TYPE_ID = "", string LOCATION_ID = "", bool PLANNED_MEMBER_INCLUDE = false, bool ESTIMATE_DISPLAY = false, string TAB_ID = "")
        {
            if (Request.IsAjaxRequest())
            {
                Condition condition = new Condition();
                condition.COMPANY_CODE = GetLoginUser().CompanyCode;
                condition.TARGET_TIME_START = TARGET_TIME_START;
                condition.TARGET_TIME_END = TARGET_TIME_END;
                condition.CONTRACT_TYPE_ID = CONTRACT_TYPE_ID;
                condition.LOCATION_ID = LOCATION_ID;

                if (!string.IsNullOrEmpty(GROUP_ID))
                {
                    condition.GROUP_ID = Convert.ToInt32(GROUP_ID);
                }

                if (!string.IsNullOrEmpty(CUSTOMER_ID))
                {
                    condition.CUSTOMER_ID = Convert.ToInt32(CUSTOMER_ID);
                }

                condition.USER_NAME = USER_NAME;
                condition.DELETE_FLG = DELETE_FLG;
                condition.PLAN_DISPLAY = PLAN_DISPLAY;
                condition.PLANNED_MEMBER_INCLUDE = PLANNED_MEMBER_INCLUDE;
                condition.ESTIMATE_DISPLAY = ESTIMATE_DISPLAY;

                var pageInfo = this.mainService.GetSalesPaymentDetailList(model, condition);
                var allData = this.mainService.GetSalesPaymentDetailListExport(condition);
                //var displayDataList = Utility.DeepClone(pageInfo.Items) as IList<PaymentSalesDetail>;

                Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = allData;

                decimal totalSalesAmount = 0;
                decimal totalCostPlan = 0;
                decimal totalCostActual = 0;
                decimal totalPaymentAmount = 0;

                foreach (var item in allData)
                {
                    totalSalesAmount += item.sales_amount;
                    totalCostPlan += Convert.ToDecimal(item.plan_cost);
                    totalCostActual += Convert.ToDecimal(item.actual_cost);
                    totalPaymentAmount += Convert.ToDecimal(item.amount);
                }

                var result = Json(new
                {
                    sEcho = model.sEcho,
                    iTotalRecords = pageInfo.TotalItems,
                    iTotalDisplayRecords = pageInfo.TotalItems,
                    aaData = (from t in pageInfo.Items
                              select new object[] {
                                t.group_name,
                                t.user_name,
                                CheckValue(t.sales_amount.ToString(), false),
                                CheckValue(t.unit_cost, false),
                                CheckWorkTime(t.plan_man_times, false),
                                CheckValue(t.plan_cost, false),
                                CheckWorkTime(t.actual_work_time, false),
                                CheckValue(t.actual_cost, false),
                                t.payment_company,
                                CheckValue(t.amount, false),
                                t.del_flg,
                                string.Format("{0}/{1:0#}",t.target_year,t.target_month),
                                t.project_name,
                                t.sales_company
                    }).ToList(),
                    totalSalesAmount,
                    totalCostPlan,
                    totalCostActual,
                    totalPaymentAmount
                }, JsonRequestBehavior.AllowGet);
                SaveRestoreData(condition);

                return result;
            }

            return new EmptyResult();
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Format string by money format
        /// </summary>
        /// <param name="value">input string</param>
        /// <param name="export"></param>
        /// <returns>formatted string</returns>
        private string CheckValue(string value, bool export = true)
        {
            return string.IsNullOrEmpty(value) ? null : Convert.ToDecimal(value).ToString("#,##0") + (export ? "" : "円");

        }

        /// <summary>
        /// Format string by work time format
        /// </summary>
        /// <param name="value">input string</param>
        /// <param name="export"></param>
        /// <returns>formatted string</returns>
        private string CheckWorkTime(string value, bool export = true)
        {
            return string.IsNullOrEmpty(value) ? null : Convert.ToDecimal(value).ToString("0.00") + (export ? "" : "h");
        }

        /// <summary>
        /// Html Encode a string
        /// </summary>
        /// <param name="value">input string</param>
        /// <returns>encoded string</returns>
        private string EncodeValue(object value)
        {
            if (value != null)
                return HttpUtility.HtmlEncode(value.ToString());

            return string.Empty;
        }

        #endregion
    }
}
