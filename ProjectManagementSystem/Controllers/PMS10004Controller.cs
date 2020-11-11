using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10004;
using ProjectManagementSystem.ViewModels;
using ProjectManagementSystem.ViewModels.PMS10004;
using ProjectManagementSystem.WorkerServices;
using ProjectManagementSystem.WorkerServices.Impl;

namespace ProjectManagementSystem.Controllers
{
    using System.Data;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.Text;

    public class PMS10004Controller : ControllerBase
    {
        private readonly IPMS10004Service _service;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS10004Controller()
            : this(new PMS10004Service())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS10004Controller(IPMS10004Service service)
        {
            this._service = service;
        }

        //
        // GET: /PMS02001/

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>List View</returns>
        public ActionResult Index()
        {
            if ((!IsInFunctionList(Constant.FunctionID.CategoryList_Admin)) && (!IsInFunctionList(Constant.FunctionID.CategoryList)))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            var model = new PMS10004ListViewModel();

            // Get Jquery data table state
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS10004/Edit") && Session[Constant.SESSION_IS_BACK] != null)
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
        /// Search customer by condition
        /// </summary>
        /// <param name="model">dataTable info</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by column</param>
        /// <param name="orderType">Order type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>Json list information</returns>
        public ActionResult Search(DataTablesModel model, Condition condition, string orderBy, string orderType, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid)
                {
                    condition.COMPANY_CODE = GetLoginUser().CompanyCode;
                    var pageInfo = this._service.Search(model, condition);
                    Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = this._service.GetListCategory(condition, orderBy, orderType);

                    var result = Json(
                        new
                        {
                            sEcho = model.sEcho,
                            iTotalRecords = pageInfo.TotalItems,
                            iTotalDisplayRecords = pageInfo.TotalItems,
                            aaData = (from t in pageInfo.Items
                                      select
                                          new object[]
                                    {
                                        t.category_id,
                                        t.peta_rn,
                                        HttpUtility.HtmlEncode(t.category), 
                                        HttpUtility.HtmlEncode(t.sub_category), 
                                        (t.upd_date != null) ? t.upd_date.ToString("yyyy/MM/dd HH:mm") : "",
                                        (t.user_update != null)? HttpUtility.HtmlEncode(t.user_update) : "",
                                        t.del_flg,
                                        t.sub_del_flg
                                    }).ToList()
                        });
                    SaveRestoreData(condition);
                    return result;
                }
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Export csv category list
        /// </summary>
        /// <param name="category"></param>
        /// <param name="sub_category"></param>
        /// <param name="deleteFlag"></param>
        /// <param name="hdnOrderBy"></param>
        /// <param name="hdnOrderType"></param>
        /// <param name="TAB_ID"></param>
        /// <returns></returns>
        public ActionResult ExportCsvListCategory(string category, string sub_category, bool deleteFlag, string hdnOrderBy, string hdnOrderType, string TAB_ID)
        {
            var condition = new Condition()
            {
                CATEGORY_NAME = category,
                SUB_CATEGORY_NAME = sub_category,
                DELETED_INCLUDE = deleteFlag,
                COMPANY_CODE = GetLoginUser().CompanyCode
            };

            if (string.IsNullOrEmpty(hdnOrderBy))
                hdnOrderBy = "upd_date";

            IList<CategoryPlus> results = new List<CategoryPlus>();
            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                results = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<CategoryPlus>;
            }
            else
            {
                results = this._service.GetListCategory(condition, hdnOrderBy, hdnOrderType);
            }

            List<CategoryPlusExport> dataExport = new List<CategoryPlusExport>();
            string[] columns = new[] {
                "No.",
                "カテゴリ",
                "サブカテゴリ",
                "更新日時",
                "更新者"
            };

            int i = 1;
            foreach (var r in results)
            {
                CategoryPlusExport tmpData = new CategoryPlusExport();
                tmpData.no = i++;
                tmpData.category = r.category;
                tmpData.sub_category = r.sub_category;
                tmpData.upd_date = r.upd_date.ToString("yyyy/MM/dd HH:mm");
                tmpData.user_update = (r.user_update != null) ? r.user_update : "";
                dataExport.Add(tmpData);
            }

            DataTable dt = Utility.ToDataTableT(dataExport, columns.ToArray());
            string fileName = "CategoryList_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";
            Utility.ExportToCsvData(this, dt, fileName);
            return new EmptyResult();
        }

        /// <summary>
        /// Edit Category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        public ActionResult Edit(int id = 0)
        {
            if (!IsInFunctionList(Constant.FunctionID.CategoryRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            var model = MakeEditViewModel(GetLoginUser().CompanyCode, id);

            return this.View("Edit", model);
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (!IsInFunctionList(Constant.FunctionID.CategoryRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            var model = MakeEditViewModel(GetLoginUser().CompanyCode, 0);

            return this.View("Edit", model);
        }

        /// <summary>
        /// Make Edit view model
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private PMS10004EditViewModel MakeEditViewModel(string company_code, int categoryId)
        {
            var model = new PMS10004EditViewModel();
            if (categoryId > 0)
            {
                model.CATEGORY = this._service.GetCategoryInfo(company_code, categoryId);
                model.LIST_SUBCATEGORY = this._service.GetListSubCategory(company_code, categoryId);
            }

            return model;
        }

        /// <summary>
        /// Get target category 
        /// </summary>
        /// <param name="subCategoryID">Sub Category Id</param>
        /// <returns>JSON list of target category </returns>
        [HttpGet]
        public ActionResult GetTargetCategory(int subCategoryID = 0, int categoryID = 0)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (subCategoryID == 0)
                return new EmptyResult();

            var targetCategory = this._service.GetTargetCategory(GetLoginUser().CompanyCode, subCategoryID, categoryID);

            JsonResult result = Json(
                targetCategory,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(PMS10004EditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginUser = GetLoginUser();
                    model.CATEGORY.upd_date = Utility.GetCurrentDateTime();
                    model.CATEGORY.upd_id = loginUser.UserId;
                    model.CATEGORY.company_code = loginUser.CompanyCode;

                    int categoryID = _service.EditCategoryData(model.CATEGORY, model.LIST_SUBCATEGORY);
                    if (categoryID > 0)
                    {
                        string action = model.CATEGORY.category_id > 0 ? "更新" : "登録";
                        string message = String.Format(Resources.Messages.I007, "カテゴリ情報", action);

                        var data = this._service.GetCategoryInfo(loginUser.CompanyCode, categoryID);
                        var subCategoryList = this._service.GetListSubCategory(loginUser.CompanyCode, categoryID);

                        foreach (var subCate in subCategoryList)
                        {
                            subCate.sub_category = HttpUtility.HtmlEncode(subCate.sub_category);
                            subCate.remarks = HttpUtility.HtmlEncode(subCate.remarks);
                            subCate.sub_category_old = subCate.sub_category;
                            subCate.remarks_old = subCate.remarks;
                        }

                        JsonResult result = Json(
                            new
                            {
                                statusCode = 201,
                                message = message,
                                categoryID = categoryID,
                                insDate = data.ins_date.ToString("yyyy/MM/dd HH:mm"),
                                updDate = data.upd_date.ToString("yyyy/MM/dd HH:mm"),
                                insUser = data.user_regist,
                                updUser = data.user_update,
                                deleted = data.del_flg.Equals(Constant.DeleteFlag.DELETE) ? true : false,
                                subCategoryList = subCategoryList
                            },
                            JsonRequestBehavior.AllowGet);

                        return result;
                    }
                    else
                    {
                        JsonResult result = Json(
                            new
                            {
                                statusCode = 500,
                                message = string.Format(Resources.Messages.E045, "カテゴリ情報")
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
                        message = string.Format(Resources.Messages.E045, "カテゴリ情報")
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
        }
    }
}
