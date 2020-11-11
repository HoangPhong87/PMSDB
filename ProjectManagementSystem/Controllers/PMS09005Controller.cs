#region License
/// <copyright file="PMS09005Controller.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2016/07/01</createdDate>
#endregion

namespace ProjectManagementSystem.Controllers
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS09002;
    using ProjectManagementSystem.ViewModels;
    using ProjectManagementSystem.ViewModels.PMS09002;
    using ProjectManagementSystem.WorkerServices;
    using ProjectManagementSystem.WorkerServices.Impl;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Work with sale list by personal
    /// </summary>
    public class PMS09005Controller : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// Common service
        /// </summary>
        private readonly IPMSCommonService commonService;

        /// <summary>
        /// Main service
        /// </summary>
        private readonly IPMS09002Service mainService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS09005Controller(IPMS09002Service service, IPMSCommonService commonservice)
        {
            this.mainService = service;
            this.commonService = commonservice;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS09005Controller()
            : this(new PMS09002Service(), new PMSCommonService())
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
            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>List View</returns>
        public ActionResult Index()
        {
            if (!this.IsInFunctionList(Constant.FunctionID.SalesNormalPersonal))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var currentUser = GetLoginUser();
            var model = new PMS09002ListViewModel
            {
                CONTRACT_TYPE_LIST = this.commonService.GetContractTypeSelectList(currentUser.CompanyCode)
            };

            var tmpCondition = GetRestoreData() as Condition;

            if (tmpCondition != null)
                model.Condition = tmpCondition;

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return this.View("List", model);
        }

        /// <summary>
        /// Search by condition
        /// </summary>
        /// <param name="model">dataTable info</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Json list information</returns>
        public ActionResult Search(DataTablesModel model, Condition condition)
        {
            if (Request.IsAjaxRequest())
            {
                DateTime from;
                DateTime to;

                if (DateTime.TryParse(condition.FROM_DATE, out from) && DateTime.TryParse(condition.TO_DATE, out to))
                {
                    condition.SORT_COL = model.iSortCol_0;
                    condition.SORT_TYPE = model.sSortDir_0;
                    string companyCode = GetLoginUser().CompanyCode;

                    var individualSalesObjList = this.mainService.GetIndividualSalesList(from, to, companyCode, condition);
                    var actualSalesObjList = this.mainService.GetActualSalesList(from, to, companyCode, condition);

                    List<IDictionary<string, object>> individualSalesTemp = new List<IDictionary<string, object>>();
                    List<IDictionary<string, object>> actualSalesTemp = new List<IDictionary<string, object>>();

                    if (individualSalesObjList != null)
                    {
                        foreach (var item in individualSalesObjList)
                        {
                            individualSalesTemp.Add(item);
                        }
                    }

                    if (actualSalesObjList != null)
                    {
                        foreach (var item in actualSalesObjList)
                        {
                            actualSalesTemp.Add(item);
                        }
                    }

                    int totalItem = individualSalesTemp.Count();
                    List<object[]> dataList = new List<object[]>();

                    if (totalItem > 0)
                    {
                        int keyCount = individualSalesTemp[0].Keys.Count;
                        int start = model.iDisplayStart;
                        int end = model.iDisplayStart + model.iDisplayLength;

                        if (end > totalItem)
                            end = totalItem;

                        for (int i = start; i < end; i++)
                        {
                            object[] obj = new object[keyCount];
                            int inc = 4;
                            string deleted = individualSalesTemp[i].Values.ElementAt(6).ToString() == Constant.DeleteFlag.DELETE ? " delete-row" : string.Empty; // is deleted data
                            string groupName = this.EncodeValue(individualSalesTemp[i].Values.ElementAt(4));
                            string userName = this.EncodeValue(individualSalesTemp[i].Values.ElementAt(5));

                            obj[0] = i + 1; // index
                            obj[1] = individualSalesTemp[i].Values.ElementAt(2); // ID
                            obj[2] = "<div class='short-text text-overflow' title='" + groupName + "'>" + groupName + "</div>";
                            obj[3] = "<div class='short-text text-overflow" + deleted + "' title='" + userName + "'>" + userName + "</div>";

                            int indexMap = -1;
                            for (int j = 0; j < actualSalesTemp.Count; j++)
                            {
                                // coincidence group ID and user ID
                                if (Convert.ToInt32(actualSalesTemp[j].Values.ElementAt(0)) == Convert.ToInt32(individualSalesTemp[i].Values.ElementAt(0))
                                    && Convert.ToInt32(actualSalesTemp[j].Values.ElementAt(1)) == Convert.ToInt32(individualSalesTemp[i].Values.ElementAt(1)))
                                {
                                    indexMap = j;
                                    break;
                                }
                            }

                            for (int k = 7; k < keyCount; k++)
                            {
                                obj[inc] = this.CheckValue(individualSalesTemp[i].Values.ElementAt(k)) + '/' + (0 > indexMap ? "0" : this.CheckValue(actualSalesTemp[indexMap].Values.ElementAt(k)));

                                inc++;
                            }

                            dataList.Add(obj);
                        }
                    }

                    var result = Json(
                        new
                        {
                            sEcho = model.sEcho,
                            iTotalRecords = totalItem,
                            iTotalDisplayRecords = totalItem,
                            aaData = dataList.ToList<object[]>()
                        },
                        JsonRequestBehavior.AllowGet);

                    SaveRestoreData(condition);

                    return result;
                }
            }

            return new EmptyResult();
        }

        #endregion

        #region Method
        /// <summary>
        /// Check value is null
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>value</returns>
        private string CheckValue(object value)
        {
            if (value != null)
                return Convert.ToString(value);

            return Constant.DEFAULT_VALUE;
        }

        /// <summary>
        /// Encode value
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>Value encoded</returns>
        private string EncodeValue(object value)
        {
            if (value != null)
                return HttpUtility.HtmlEncode(value.ToString());

            return string.Empty;
        }

        #endregion
    }
}
