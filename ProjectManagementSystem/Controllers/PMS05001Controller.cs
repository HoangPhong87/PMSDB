#region License
/// <copyright file="PMS05001Controller.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/22</createdDate>
#endregion

namespace ProjectManagementSystem.Controllers
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS05001;
    using ProjectManagementSystem.ViewModels;
    using ProjectManagementSystem.ViewModels.PMS05001;
    using ProjectManagementSystem.WorkerServices;
    using ProjectManagementSystem.WorkerServices.Impl;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Work with group
    /// </summary>
    public class PMS05001Controller : ControllerBase
    {
        #region Constructor
        /// Main service
        private readonly IPMS05001Service mainService;

        /// Common service
        private readonly IPMSCommonService commonService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS05001Controller(IPMS05001Service service, IPMSCommonService cmService)
        {
            this.mainService = service;
            this.commonService = cmService;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS05001Controller()
            : this(new PMS05001Service(), new PMSCommonService())
        {
        }

        #endregion

        #region Action
        /// <summary>
        /// Index
        /// </summary>
        /// <returns>List View</returns>
        public ActionResult Index()
        {
            if (!this.IsInFunctionList(Constant.FunctionID.GroupList_Admin)
                && !this.IsInFunctionList(Constant.FunctionID.GroupList))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = new PMS05001ListViewModel();

            // Get Jquery data table state
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS05001/Edit") && Session[Constant.SESSION_IS_BACK] != null)
            {
                var tmpCondition = GetRestoreData() as Condition;

                if (tmpCondition != null)
                    model.Condition = tmpCondition;
            }

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return this.View("List", model);
        }

        /// <summary>
        /// Export group list to csv file
        /// </summary>
        /// <param name="hdnGroupName">Group name</param>
        /// <param name="hdnDelFlag">Group delete flag</param>
        /// <param name="hdnOrderBy">Group list order by</param>
        /// <param name="hdnOrderType">Group list order type</param>
        /// <returns>csv file</returns>
        public ActionResult ExportCsv(string hdnGroupName, bool hdnDelFlag, string hdnOrderBy, string hdnOrderType, string TAB_ID)
        {
            try
            {
                Condition condition = new Condition()
                {
                    GroupName = hdnGroupName,
                    DeleteFlag = hdnDelFlag
                };
                List<string> titles = new List<string>()
                {
                    "No.",
                    "所属名",
                    "所属",
                    "予算対象",
                    "備考",
                    "更新日時",
                    "更新者"
                };

                if (string.IsNullOrEmpty(hdnOrderBy))
                    hdnOrderBy = "upd_date";

                string fileName = "GroupList_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";

                IList<GroupPlus> groupList = new List<GroupPlus>();
                if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
                {
                    groupList = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<GroupPlus>;
                }
                else
                {
                    groupList = this.mainService.ExportGroupListToCSV(GetLoginUser().CompanyCode, condition, hdnOrderBy, hdnOrderType);
                }

                List<GroupListExportCSV> dataExport = new List<GroupListExportCSV>();

                foreach (var groupInfo in groupList)
                {
                    dataExport.Add(new GroupListExportCSV()
                    {
                        peta_rn = groupInfo.peta_rn.ToString(),
                        group_name = groupInfo.group_name,
                        display_name = groupInfo.display_name,
                        budget_setting_flg = (groupInfo.budget_setting_flg.Equals(Constant.DEFAULT_VALUE) ? "無" : "有"),
                        remarks = (string.IsNullOrEmpty(groupInfo.remarks) ? "" : groupInfo.remarks.Replace("\r\n", " ")),
                        upd_date = groupInfo.upd_date.Value.ToString("yyyy/MM/dd HH:mm"),
                        upd_user = groupInfo.upd_user
                    });
                }

                DataTable dt = Utility.ToDataTableT(dataExport, titles.ToArray());
                Utility.ExportToCsvData(this, dt, fileName);

                return new EmptyResult();
            }
            catch
            {
                return this.RedirectToAction("Index", "Error");
            }
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="id">Group ID</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        public ActionResult Edit(int id = 0)
        {
            if (!this.IsInFunctionList(Constant.FunctionID.GroupRegist))
                return this.RedirectToAction("Index", "ErrorAuthent");

            var model = new PMS05001EditViewModel();
            model.CheckActualWorkFlag = true;

            if (id > 0)
            {
                model.GroupInfo = this.mainService.GetGroupInfo(GetLoginUser().CompanyCode, id);
            }

            return this.View("Edit", model);
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (!this.IsInFunctionList(Constant.FunctionID.GroupRegist))
                return this.RedirectToAction("Index", "ErrorAuthent");

            var model = new PMS05001EditViewModel();

            return this.View("Edit", model);
        }

        /// <summary>
        /// Edit group info
        /// </summary>
        /// <param name="model">Group info to edit</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGroup(PMS05001EditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginUser = GetLoginUser();

                    // Check limit data by license of company
                    if ((model.GroupInfo.group_id == 0
                        || (model.OLD_DEL_FLAG
                        && Constant.DeleteFlag.NON_DELETE.Equals(model.GroupInfo.del_flg)))
                        && !this.commonService.CheckValidUpdateData(loginUser.CompanyCode, Constant.LicenseDataType.GROUP))
                    {
                        JsonResult result = Json(
                            new
                            {
                                statusCode = 500,
                                message = string.Format(Resources.Messages.E067, "所属")
                            },
                            JsonRequestBehavior.AllowGet);

                        return result;
                    }

                    model.GroupInfo.upd_date = Utility.GetCurrentDateTime();
                    model.GroupInfo.upd_id = loginUser.UserId;
                    model.GroupInfo.company_code = loginUser.CompanyCode;

                    int groupID = 0;

                    if (this.mainService.EditGroupInfo(model.GroupInfo, out groupID))
                    {
                        string action = model.GroupInfo.group_id > 0 ? "更新" : "登録";
                        string message = String.Format(Resources.Messages.I007, "所属情報", action);
                        var data = this.mainService.GetGroupInfo(loginUser.CompanyCode, groupID);

                        JsonResult result = Json(
                            new
                            {
                                statusCode = 201,
                                message = message,
                                groupID = groupID,
                                insDate = data.ins_date.Value.ToString("yyyy/MM/dd HH:mm"),
                                updDate = data.upd_date.Value.ToString("yyyy/MM/dd HH:mm"),
                                insUser = data.ins_user,
                                updUser = data.upd_user,
                                deleted = data.del_flg.Equals(Constant.DeleteFlag.DELETE) ? true : false
                            },
                            JsonRequestBehavior.AllowGet);

                        return result;
                    }
                    else
                    {
                        ModelState.AddModelError("", Resources.Messages.E001);
                        JsonResult result = Json(
                            new
                            {
                                statusCode = 500,
                                message = string.Format(Resources.Messages.E045, "所属情報")
                            },
                            JsonRequestBehavior.AllowGet);

                        return result;
                    }
                }

                return new EmptyResult();
            }
            catch
            {
                JsonResult result = Json(
                    new
                    {
                        statusCode = 500,
                        message = string.Format(Resources.Messages.E045, "所属情報")
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
        }

        #endregion

        #region Ajax Action
        /// <summary>
        /// Search group by condition
        /// </summary>
        /// <param name="model">dataTable info</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Json list of group</returns>
        public ActionResult Search(DataTablesModel model, Condition condition, string orderBy, string orderType, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                var pageInfo = this.mainService.Search(model, GetLoginUser().CompanyCode, condition);

                Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = this.mainService.ExportGroupListToCSV(GetLoginUser().CompanyCode, condition, orderBy, orderType);

                var result = Json(
                    new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = pageInfo.TotalItems,
                        iTotalDisplayRecords = pageInfo.TotalItems,
                        aaData = (from t in pageInfo.Items select new object[]
                        {
                            t.group_id,
                            t.peta_rn,
                            HttpUtility.HtmlEncode(t.group_name),
                            HttpUtility.HtmlEncode(t.display_name),
                            (t.budget_setting_flg.Equals(Constant.DEFAULT_VALUE) ? "無" : "有"),
                            HttpUtility.HtmlEncode(t.remarks),
                            t.upd_date.Value.ToString("yyyy/MM/dd HH:mm"),
                            HttpUtility.HtmlEncode(t.upd_user),
                            t.del_flg
                        }).ToList()
                    },
                    JsonRequestBehavior.AllowGet);
                SaveRestoreData(condition);
                return result;
            }

            return new EmptyResult();
        }

        #endregion
    }
}
