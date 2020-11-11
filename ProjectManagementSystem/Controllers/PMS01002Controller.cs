using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS01002;
using ProjectManagementSystem.ViewModels;
using ProjectManagementSystem.ViewModels.PMS01002;
using ProjectManagementSystem.WorkerServices;
using ProjectManagementSystem.WorkerServices.Impl;
using System.Text.RegularExpressions;

namespace ProjectManagementSystem.Controllers
{
    public class PMS01002Controller : ControllerBase
    {
        private readonly IPMSCommonService commonService;

        private readonly IPMS01002Service _service;

        /// <summary>
        /// TempData storage
        /// </summary>
        [System.Serializable]
        private class TmpValues
        {
            public int UserID { get; set; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS01002Controller()
            : this(new PMS01002Service(), new PMSCommonService())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS01002Controller(IPMS01002Service service, IPMSCommonService commonservice)
        {
            this._service = service;
            this.commonService = commonservice;
        }

        /// <summary>
        /// Function index to load list User
        /// </summary>
        /// <returns>List View</returns>
        public ActionResult Index()
        {
            if ((!IsInFunctionList(Constant.FunctionID.UserList_Admin)) && (!IsInFunctionList(Constant.FunctionID.UserList)))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            string companyCode = GetLoginUser().CompanyCode;
            var model = new PMS01002ListViewModel
            {
                GROUP_LIST = this.commonService.GetUserGroupSelectList(companyCode),
                POSITION_LIST = this.GetPositionList(companyCode)
            };

            // Get Jquery data table state
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS01002/Edit") && Session[Constant.SESSION_IS_BACK] != null)
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
        /// Function call ajax to load list User
        /// </summary>
        /// <param name="model">DataTablesModel</param>
        /// <param name="condition">Condition</param>
        /// <returns>Json User List</returns>
        public ActionResult Search(DataTablesModel model, Condition condition, string hdnOrderBy, string hdnOrderType, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid)
                {
                    var pageInfo = this._service.Search(model, condition, GetLoginUser().CompanyCode);

                    if (!String.IsNullOrEmpty(TAB_ID)) // if current screen is User list
                    {
                        Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = this._service.GetListUser(condition, GetLoginUser().CompanyCode, hdnOrderBy, hdnOrderType);
                    }

                    var result = Json(
                        new
                        {
                            sEcho = model.sEcho,
                            iTotalRecords = pageInfo.TotalItems,
                            iTotalDisplayRecords = pageInfo.TotalItems,
                            aaData = (from t in pageInfo.Items
                                      select new object[]
                                    {
                                        t.user_sys_id,
                                        t.peta_rn,
                                        HttpUtility.HtmlEncode(t.display_name),
                                        HttpUtility.HtmlEncode(t.display_name_group),
                                        HttpUtility.HtmlEncode(t.display_name_position),
                                        t.base_unit_cost != null ? t.base_unit_cost.Value.ToString("#,##0") : "-",
                                        (t.entry_date != null)?t.entry_date.Value.ToString("yyyy/MM/dd") : "",
                                        (t.mail_address_1 != null)? HttpUtility.HtmlEncode(t.mail_address_1) : "",
                                        t.is_active,
                                        (t.upd_date != null)?t.upd_date.ToString("yyyy/MM/dd HH:mm") : "",
                                        (t.user_update != null)? HttpUtility.HtmlEncode(t.user_update) : "",
                                        t.user_sys_id,
                                        t.del_flg,
                                        t.group_id
                                    }).ToList()
                        });
                    SaveRestoreData(condition);
                    return result;
                }
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Export to csv user list
        /// </summary>
        /// <param name="search_userName"></param>
        /// <param name="search_groupId"></param>
        /// <param name="search_positionId"></param>
        /// <param name="search_email"></param>
        /// <param name="search_deleteFlag"></param>
        /// <param name="search_retirementFlag"></param>
        /// <param name="hdnOrderBy"></param>
        /// <param name="hdnOrderType"></param>
        /// <returns></returns>
        public ActionResult ExportCsvListUser(string search_userName, string search_groupId, string search_positionId, string search_email, bool search_deleteFlag, bool search_retirementFlag, string hdnOrderBy, string hdnOrderType, string TAB_ID)
        {
            Condition condition = new Condition();
            condition.DISPLAY_NAME = search_userName;
            if (!string.IsNullOrEmpty(search_groupId))
            {
                condition.GROUP_ID = Convert.ToInt32(search_groupId);
            }

            if (!string.IsNullOrEmpty(search_positionId))
            {
                condition.POSITION_ID = Convert.ToInt32(search_positionId);
            }

            condition.MAIL_ADDRESS = search_email;
            condition.DELETED_INCLUDE = search_deleteFlag;
            condition.RETIREMENT_INCLUDE = search_retirementFlag;

            if (string.IsNullOrEmpty(hdnOrderBy))
                hdnOrderBy = "upd_date";

            IList<UserPlus> results = new List<UserPlus>();
            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                results = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<UserPlus>;
            }
            else
            {
                results = this._service.GetListUser(condition, GetLoginUser().CompanyCode, hdnOrderBy, hdnOrderType);
            }

            List<UserPlusExport> dataExport = new List<UserPlusExport>();
            string[] columns = new[] {
                    "No.",
                    "ユーザー名",
                    "社員No",
                    "所属",
                    "役職",
                    "基準単価",
                    "入社年月日",
                    "メールアドレス",
                    "有効/無効",
                    "更新日時",
                    "更新者"
            };

            for (int i = 0; i < results.Count; i++)
            {
                results[i].peta_rn = i + 1;
            }

            foreach (var t in results)
            {
                UserPlusExport tmpData = new UserPlusExport();
                tmpData.user_id = t.peta_rn;
                tmpData.display_name = t.display_name;
                tmpData.employee_no = t.employee_no;
                tmpData.display_name_group = t.display_name_group;
                tmpData.display_name_position = t.display_name_position;
                tmpData.base_unit_cost = (t.base_unit_cost != null) ? t.base_unit_cost.Value.ToString("#,##0円") : "-";
                tmpData.entry_date = (t.entry_date != null) ? t.entry_date.Value.ToString("yyyy/MM/dd") : "";
                tmpData.mail_address = (t.mail_address_1 != null) ? t.mail_address_1 : "";
                tmpData.is_active = t.is_active;
                tmpData.upd_date = (t.upd_date != null) ? t.upd_date.ToString("yyyy/MM/dd HH:mm") : "";
                tmpData.user_update = (t.user_update != null) ? t.user_update : "";
                dataExport.Add(tmpData);
            }

            DataTable dt = Utility.ToDataTableT(dataExport, columns.ToArray());
            string fileName = "UserList_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";
            Utility.ExportToCsvData(this, dt, fileName);

            return new EmptyResult();
        }

        /// <summary>
        /// Function Edit infomation of user (case regist id = 0)
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Edit View</returns>
        [HttpPost]
        public ActionResult Edit(int id = 0)
        {
            if (!IsInFunctionList(Constant.FunctionID.UserRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = MakeEditViewModel(id);

            return this.View("Edit", model);
        }

        /// <summary>
        /// Function Edit infomation of user (case update id != 0)
        /// </summary>
        /// <returns>Edit View</returns>
        [HttpGet]
        public ActionResult Edit()
        {
            if (!IsInFunctionList(Constant.FunctionID.UserRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = MakeEditViewModel(0);

            return this.View("Edit", model);
        }

        /// <summary>
        /// Function create object PMS01002EditViewModel
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>Edit View Model</returns>
        private PMS01002EditViewModel MakeEditViewModel(int userId)
        {
            string companyCode = GetLoginUser().CompanyCode;
            var model = new PMS01002EditViewModel();

            model.GROUP_LIST = this.commonService.GetUserGroupSelectList(companyCode);
            model.POSITION_LIST = this.GetPositionList(companyCode);
            model.AUTHORITYROLE_LIST = this.GetAuthorityRoleList(companyCode);
            model.BRANCH_LIST = this.GetBranchList(companyCode);
            if (userId > 0)
            {
                model.USER_INFO = this._service.GetUserInfo(companyCode, userId);
                model.USER_INFO.user_regist = model.USER_INFO.user_regist;
                model.USER_INFO.user_update = model.USER_INFO.user_update;
                model.USER_INFO.unit_price_history = this._service.GetUnitPriceHistoryInfo(companyCode, userId);
                model.data_editable_time = this._service.GetDataEditTableTime(companyCode);
            }
            return model;
        }

        /// <summary>
        /// Function Edit infomation of user [POST]
        /// </summary>
        /// <param name="model">PMS01002EditViewModel</param>
        /// <returns>Edit User</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(PMS01002EditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var tmp_user_sys_id = model.USER_INFO.user_sys_id;

                    //Check min lengthe of password
                    if (model.USER_INFO.password != Constant.DISPLAY_PASSWORD)
                    {
                        if (model.USER_INFO.password.Length < 6)
                        {
                            model = MakeEditViewModel(model.USER_INFO.user_sys_id);
                            ModelState.AddModelError("", string.Format(Resources.Messages.E023, "パスワード"));
                            return new EmptyResult();
                        }

                        if (!Regex.IsMatch(model.USER_INFO.password, Constant.REG_PASSWORD))
                        {
                            model = MakeEditViewModel(model.USER_INFO.user_sys_id);
                            ModelState.AddModelError("", string.Format(Resources.Messages.E003, "パスワード"));
                            return new EmptyResult();
                        }
                    }

                    HttpPostedFileBase file = Request.Files["file"];
                    HttpPostedFileBase fileDrag = Request.Files["fileDrag"];

                    if (model.TypeUpload == "file" && file != null && file.FileName.Length > 0)
                    {
                        if (!Constant.AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                        {
                            model = MakeEditViewModel(model.USER_INFO.user_sys_id);
                            ModelState.AddModelError("", String.Format(Resources.Messages.E010, "jpg,png.jpeg"));
                            return new EmptyResult();
                        }

                        if (file.ContentLength > Constant.MaxContentLength)
                        {
                            model = MakeEditViewModel(model.USER_INFO.user_sys_id);
                            ModelState.AddModelError("", String.Format(Resources.Messages.E021, "500KB以内"));
                            return new EmptyResult();
                        }

                        model.USER_INFO.image_file_path = UploadFile.UploadFiles(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH], file, ConfigurationManager.AppSettings[ConfigurationKeys.TEMP_USER_PATH]);
                    }
                    else if (model.TypeUpload == "fileDrag" && fileDrag != null && fileDrag.FileName.Length > 0)
                    {
                        file = fileDrag;

                        if (!Constant.AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                        {
                            model = MakeEditViewModel(model.USER_INFO.user_sys_id);
                            ModelState.AddModelError("", String.Format(Resources.Messages.E010, "jpg,png.jpeg"));
                            return new EmptyResult();
                        }

                        if (file.ContentLength > Constant.MaxContentLength)
                        {
                            model = MakeEditViewModel(model.USER_INFO.user_sys_id);
                            ModelState.AddModelError("", String.Format(Resources.Messages.E021, "500KB以内"));
                            return new EmptyResult();
                        }

                        model.USER_INFO.image_file_path = UploadFile.UploadFiles(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH], file, ConfigurationManager.AppSettings[ConfigurationKeys.TEMP_USER_PATH]);
                    }
                    else
                    {
                        if (model.Clear == "1")
                        {
                            model.USER_INFO.image_file_path = string.Empty;
                        }
                    }

                    var loginUser = this.GetLoginUser();

                    model.USER_INFO.upd_date = Utility.GetCurrentDateTime();
                    model.USER_INFO.upd_id = loginUser.UserId;
                    model.USER_INFO.company_code = loginUser.CompanyCode;

                    if (model.USER_INFO.password != Constant.DISPLAY_PASSWORD)
                    {
                        model.USER_INFO.password = SafePassword.GetSaltedPassword(model.USER_INFO.password);
                        var user = _service.CheckPassword(model.USER_INFO.user_account, loginUser.CompanyCode, model.USER_INFO.user_sys_id);
                        if (user != null && model.USER_INFO.password == user.password)
                        {
                            model = MakeEditViewModel(model.USER_INFO.user_sys_id);
                            ModelState.AddModelError("", string.Format(Resources.Messages.E053));
                            return new EmptyResult();
                        }
                    }

                    if (!string.IsNullOrEmpty(model.USER_INFO.mail_address_1) || !string.IsNullOrEmpty(model.USER_INFO.mail_address_2))
                    {
                        if (model.USER_INFO.mail_address_1.Trim() == model.USER_INFO.mail_address_2.Trim())
                        {
                            model = MakeEditViewModel(model.USER_INFO.user_sys_id);
                            ModelState.AddModelError("", String.Format(Resources.Messages.E008, "メールアドレス", "メールアドレス"));
                            return new EmptyResult();
                        }

                        if (_service.CheckUserEmail(model.USER_INFO.mail_address_1, model.USER_INFO.mail_address_2, model.USER_INFO.user_sys_id, loginUser.CompanyCode) > 0)
                        {
                            model = MakeEditViewModel(model.USER_INFO.user_sys_id);
                            ModelState.AddModelError("", String.Format(Resources.Messages.E008, "メールアドレス", "メールアドレス"));
                            return new EmptyResult();
                        }
                    }

                    if (_service.CheckUserAccount(model.USER_INFO.user_account, loginUser.CompanyCode, model.USER_INFO.user_sys_id) > 0)
                    {
                        model = MakeEditViewModel(model.USER_INFO.user_sys_id);
                        ModelState.AddModelError("", String.Format(Resources.Messages.E008, "ユーザーアカウント", "ユーザーアカウント"));
                        return new EmptyResult();
                    }

                    if ((model.USER_INFO.user_sys_id == 0
                        || (model.OLD_DEL_FLAG
                        && Constant.DeleteFlag.NON_DELETE.Equals(model.USER_INFO.del_flg)))
                        && !this.commonService.CheckValidUpdateData(loginUser.CompanyCode, Constant.LicenseDataType.USER))
                    {
                        JsonResult result = Json(
                            new
                            {
                                statusCode = 500,
                                message = string.Format(Resources.Messages.E067, "ユーザー")
                            },
                            JsonRequestBehavior.AllowGet);

                        return result;
                    }

                    int userId = _service.EditUserData(model.USER_INFO);
                    if (userId > 0)
                    {
                        if (file != null && file.FileName.Length > 0)
                        {
                            UploadFile.CreateFolder(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH] + ConfigurationManager.AppSettings[ConfigurationKeys.USER_PATH] + "/" + loginUser.CompanyCode + "/" + userId.ToString());
                            model.USER_INFO.user_sys_id = userId;
                            if (model.USER_INFO.del_flg == null)
                            {
                                model.USER_INFO.del_flg = Constant.DeleteFlag.NON_DELETE;
                            }
                            model.USER_INFO.image_file_path = ConfigurationManager.AppSettings[ConfigurationKeys.USER_PATH] + "/" + loginUser.CompanyCode + "/" + userId + "/" + ConfigurationManager.AppSettings[ConfigurationKeys.PROFILE_IMAGE] + file.FileName.Substring(file.FileName.LastIndexOf('.'));
                            model.USER_INFO.row_version = this._service.GetUserInfo(loginUser.CompanyCode, userId).row_version;
                            if (_service.EditUserData(model.USER_INFO) > 0)
                            {
                                // Move image
                                UploadFile.MoveFile(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH] + ConfigurationManager.AppSettings[ConfigurationKeys.TEMP_USER_PATH] + "/" +
                                    file.FileName, ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH] + ConfigurationManager.AppSettings[ConfigurationKeys.USER_PATH] + "/" + loginUser.CompanyCode + "/" + userId + "/" + ConfigurationManager.AppSettings[ConfigurationKeys.PROFILE_IMAGE] + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                            }
                        }

                        model.USER_INFO.user_sys_id = userId; //update new user_id to model

                        //update unit price history
                        if (_service.UpdateUnitPriceHistory(model.USER_INFO, model.USER_INFO.upd_id) > 0)
                        {
                            var sessionLogin = Session[Constant.SESSION_LOGIN_USER] as LoginUser;
                            if (sessionLogin.UserId == userId && sessionLogin.DisplayName != model.USER_INFO.display_name)
                            {
                                sessionLogin.DisplayName = model.USER_INFO.display_name;
                            }

                            if (sessionLogin.UserId == userId && sessionLogin.ImageFilePath != model.USER_INFO.image_file_path)
                            {
                                sessionLogin.ImageFilePath = model.USER_INFO.image_file_path;
                            }

                            if (sessionLogin.UserId == userId
                               && sessionLogin.Password != model.USER_INFO.password
                               && model.USER_INFO.password != Constant.DISPLAY_PASSWORD)
                            {
                                sessionLogin.Password = model.USER_INFO.password;
                                sessionLogin.Is_expired_password = false;
                            }

                            if (sessionLogin.UserId == userId && sessionLogin.ActualWorkInputMode != model.USER_INFO.actual_work_input_mode)
                            {
                                sessionLogin.ActualWorkInputMode = model.USER_INFO.actual_work_input_mode;
                            }
                            SetLoginUser(sessionLogin);

                            string action = Convert.ToInt32(tmp_user_sys_id) > 0 ? "更新" : "登録";
                            string message = string.Format(Resources.Messages.I007, "ユーザー情報", action);

                            var data = this._service.GetUserInfo(loginUser.CompanyCode, userId);

                            JsonResult result = Json(
                                    new
                                    {
                                        statusCode = 201,
                                        message = message,
                                        id = userId,
                                        row_version = Convert.ToBase64String(data.row_version),
                                        insDate = (data.ins_date != null) ? data.ins_date.ToString("yyyy/MM/dd HH:mm") : "",
                                        updDate = (data.upd_date != null) ? data.upd_date.ToString("yyyy/MM/dd HH:mm") : "",
                                        insUser = data.user_regist,
                                        updUser = data.user_update,
                                        deleted = data.del_flg.Equals(Constant.DeleteFlag.DELETE) ? true : false,
                                        imageFilePath = data.image_file_path,
                                        userIDSesssion = loginUser.UserId,
                                        userNameSesssion = sessionLogin.DisplayName
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
                                            message = string.Format(Resources.Messages.E045, "ユーザー情報")
                                        },
                                        JsonRequestBehavior.AllowGet);

                            return result;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", Resources.Messages.E001);

                        JsonResult result = Json(
                                    new
                                    {
                                        statusCode = 500,
                                        message = string.Format(Resources.Messages.E045, "ユーザー情報")
                                    },
                                    JsonRequestBehavior.AllowGet);

                        return result;
                    }
                }

                return new EmptyResult();
            }
            catch (Exception)
            {
                JsonResult result = Json(
                    new
                    {
                        statusCode = 500,
                        message = string.Format(Resources.Messages.E045, "ユーザー情報")
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
        }

        /// <summary>
        /// Personal Setting
        /// </summary>
        /// <returns>Personal Setting view</returns>
        public ActionResult PersonalSetting()
        {
            if (Session[Constant.PASSWORD_OUT_OF_DATE] != null)
            {
                ViewBag.PasswordOutOfDate = true;
                Session[Constant.PASSWORD_OUT_OF_DATE] = null;
            }
            if (!IsInFunctionList(Constant.FunctionID.PersonalSetting))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            var model = MakePersonalSettingViewModel(GetLoginUser().UserId);

            return this.View("PersonalSetting", model);
        }

        /// <summary>
        /// Check data by search condition
        /// </summary>
        /// <param name="condition">Search condition</param>
        /// <param name="user">User object</param>
        /// <returns>bool:true/false</returns>
        private bool checkSearchCondition(Condition condition, UserPlus user)
        {
            bool check = true;

            if (condition != null)
            {
                if (!string.IsNullOrEmpty(condition.DISPLAY_NAME)) // check name
                {
                    user.display_name = !string.IsNullOrEmpty(user.display_name) ? user.display_name : string.Empty;
                    user.user_name_sei = !string.IsNullOrEmpty(user.user_name_sei) ? user.user_name_sei : string.Empty;
                    user.user_name_mei = !string.IsNullOrEmpty(user.user_name_mei) ? user.user_name_mei : string.Empty;
                    user.furigana_sei = !string.IsNullOrEmpty(user.furigana_sei) ? user.furigana_sei : string.Empty;
                    user.furigana_mei = !string.IsNullOrEmpty(user.furigana_mei) ? user.furigana_mei : string.Empty;

                    if (user.display_name.IndexOf(condition.DISPLAY_NAME) != -1
                        || user.user_name_sei.IndexOf(condition.DISPLAY_NAME) != -1
                        || user.user_name_mei.IndexOf(condition.DISPLAY_NAME) != -1
                        || user.furigana_sei.IndexOf(condition.DISPLAY_NAME) != -1
                        || user.furigana_mei.IndexOf(condition.DISPLAY_NAME) != -1)
                    {
                        check = true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (condition.GROUP_ID != null) // check group
                {
                    if (condition.GROUP_ID != user.group_id)
                    {
                        return false;
                    }
                }

                if (condition.POSITION_ID != null) // check position
                {
                    if (condition.POSITION_ID != user.position_id)
                    {
                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(condition.MAIL_ADDRESS)) // check mail address
                {
                    user.mail_address_1 = !string.IsNullOrEmpty(user.mail_address_1) ? user.mail_address_1 : string.Empty;
                    user.mail_address_2 = !string.IsNullOrEmpty(user.mail_address_2) ? user.mail_address_2 : string.Empty;

                    if (user.mail_address_1.IndexOf(condition.MAIL_ADDRESS) != -1
                        || user.mail_address_2.IndexOf(condition.MAIL_ADDRESS) != -1)
                    {
                        check = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return check;
        }

        /// <summary>
        /// Function Create object PMS01002PersonalSettingViewModel
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>Personal Setting View Model</returns>
        private PMS01002PersonalSettingViewModel MakePersonalSettingViewModel(int userId)
        {
            var model = new PMS01002PersonalSettingViewModel();
            string companyCode = GetLoginUser().CompanyCode;

            model.GROUP_LIST = this.commonService.GetUserGroupSelectList(companyCode);
            model.POSITION_LIST = this.GetPositionList(companyCode);
            model.LANGUAGE_LIST = this.GetLanguageList();
            if (userId > 0)
            {
                model.USER_INFO = this._service.GetUserInfo(companyCode, userId);
                model.USER_INFO.user_account = HttpUtility.HtmlEncode(model.USER_INFO.user_account);
            }

            return model;
        }

        /// <summary>
        /// Function Update infomation of user
        /// </summary>
        /// <param name="model">PMS01002PersonalSettingViewModel</param>
        /// <returns>Personal Setting View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PersonalSetting(PMS01002PersonalSettingViewModel model)
        {
            try
            {
                int userId = GetLoginUser().UserId;
                if (userId > 0)
                    model.USER_INFO.user_sys_id = userId;

                if (ModelState.IsValid)
                {
                    if (model.USER_INFO.password != Constant.DISPLAY_PASSWORD)
                    {
                        if (model.USER_INFO.password.Length < 6)
                        {
                            model = MakePersonalSettingViewModel(userId);
                            ModelState.AddModelError("", string.Format(Resources.Messages.E023, "パスワード"));
                            return new EmptyResult();
                        }

                        if (!Regex.IsMatch(model.USER_INFO.password, Constant.REG_PASSWORD))
                        {
                            model = MakePersonalSettingViewModel(userId);
                            ModelState.AddModelError("", string.Format(Resources.Messages.E003, "パスワード"));
                            return new EmptyResult();
                        }
                    }

                    if (model.confirmPassword != Constant.DISPLAY_PASSWORD)
                    {
                        if (model.confirmPassword.Length < 6)
                        {
                            model = MakePersonalSettingViewModel(userId);
                            ModelState.AddModelError("", string.Format(Resources.Messages.E023, "パスワード（確認用）"));
                            return new EmptyResult();
                        }

                        if (!Regex.IsMatch(model.confirmPassword, Constant.REG_PASSWORD))
                        {
                            model = MakePersonalSettingViewModel(userId);
                            ModelState.AddModelError("", string.Format(Resources.Messages.E003, "パスワード（確認用）"));
                            return new EmptyResult();
                        }
                    }

                    if (model.USER_INFO.password != model.confirmPassword)
                    {
                        model = MakePersonalSettingViewModel(userId);
                        ModelState.AddModelError("", string.Format(Resources.Messages.E048));
                        return new EmptyResult();
                    }

                    var sessionLogin = Session[Constant.SESSION_LOGIN_USER] as LoginUser;
                    if (model.USER_INFO.password != Constant.DISPLAY_PASSWORD)
                    {
                        if (SafePassword.GetSaltedPassword(model.USER_INFO.password) == sessionLogin.Password)
                        {
                            model = MakePersonalSettingViewModel(userId);
                            ModelState.AddModelError("", string.Format(Resources.Messages.E053));
                            return new EmptyResult();
                        }
                    }

                    HttpPostedFileBase file = Request.Files["file"];
                    HttpPostedFileBase fileDrag = Request.Files["fileDrag"];

                    if (model.TypeUpload == "file" && file != null && file.FileName.Length > 0)
                    {
                        if (!Constant.AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                        {
                            model = MakePersonalSettingViewModel(userId);
                            ModelState.AddModelError("", String.Format(Resources.Messages.E010, "jpg,png.jpeg"));
                            return new EmptyResult();
                        }

                        if (file.ContentLength > Constant.MaxContentLength)
                        {
                            model = MakePersonalSettingViewModel(userId);
                            ModelState.AddModelError("", String.Format(Resources.Messages.E021, "500KB以内"));
                            return new EmptyResult();
                        }

                        model.USER_INFO.image_file_path = UploadFile.UploadFiles(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH], file, ConfigurationManager.AppSettings[ConfigurationKeys.TEMP_USER_PATH]);
                    }
                    else if (model.TypeUpload == "fileDrag" && fileDrag != null && fileDrag.FileName.Length > 0)
                    {
                        file = fileDrag;
                        if (!Constant.AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                        {
                            model = MakePersonalSettingViewModel(userId);
                            ModelState.AddModelError("", String.Format(Resources.Messages.E010, "jpg,png.jpeg"));
                            return new EmptyResult();
                        }

                        if (file.ContentLength > Constant.MaxContentLength)
                        {
                            model = MakePersonalSettingViewModel(userId);
                            ModelState.AddModelError("", String.Format(Resources.Messages.E021, "500KB以内"));
                            return new EmptyResult();
                        }

                        model.USER_INFO.image_file_path = UploadFile.UploadFiles(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH], file, ConfigurationManager.AppSettings[ConfigurationKeys.TEMP_USER_PATH]);
                    }
                    else
                    {
                        if (model.Clear == "1")
                        {
                            model.USER_INFO.image_file_path = string.Empty;
                        }
                    }

                    model.USER_INFO.upd_date = Utility.GetCurrentDateTime();
                    model.USER_INFO.upd_id = userId;
                    model.USER_INFO.company_code = GetLoginUser().CompanyCode;
                    model.USER_INFO.password_lock_flg = Constant.PasswordLockFlag.NON_LOCK;
                    model.USER_INFO.language_id = (model.USER_INFO.language_id != null) ? model.USER_INFO.language_id : 0;

                    if (model.USER_INFO.password != Constant.DISPLAY_PASSWORD)
                    {
                        model.USER_INFO.password = SafePassword.GetSaltedPassword(model.USER_INFO.password);
                    }

                    if (!string.IsNullOrEmpty(model.USER_INFO.mail_address_1) || !string.IsNullOrEmpty(model.USER_INFO.mail_address_2))
                    {
                        if (model.USER_INFO.mail_address_1.Trim() == model.USER_INFO.mail_address_2.Trim())
                        {
                            model = MakePersonalSettingViewModel(userId);
                            ModelState.AddModelError("", String.Format(Resources.Messages.E008, "メールアドレス", "メールアドレス"));
                            return new EmptyResult();
                        }

                        if (_service.CheckUserEmail(model.USER_INFO.mail_address_1, model.USER_INFO.mail_address_2, model.USER_INFO.user_sys_id, GetLoginUser().CompanyCode) > 0)
                        {
                            model = MakePersonalSettingViewModel(userId);
                            ModelState.AddModelError("", String.Format(Resources.Messages.E008, "メールアドレス", "メールアドレス"));
                            return new EmptyResult();
                        }
                    }

                    if (_service.PersonalSettingUserData(model.USER_INFO) > 0)
                    {
                        if (file != null && file.FileName.Length > 0)
                        {
                            UploadFile.CreateFolder(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH] + ConfigurationManager.AppSettings[ConfigurationKeys.USER_PATH] + "/" + GetLoginUser().CompanyCode + "/" + userId.ToString());
                            model.USER_INFO.user_sys_id = userId;
                            model.USER_INFO.del_flg = Constant.DeleteFlag.NON_DELETE;
                            model.USER_INFO.image_file_path = ConfigurationManager.AppSettings[ConfigurationKeys.USER_PATH] + "/" + GetLoginUser().CompanyCode + "/" + userId + "/" + ConfigurationManager.AppSettings[ConfigurationKeys.PROFILE_IMAGE] + file.FileName.Substring(file.FileName.LastIndexOf('.'));
                            model.USER_INFO.row_version = this._service.GetUserInfo(GetLoginUser().CompanyCode, userId).row_version;
                            if (_service.PersonalSettingUserData(model.USER_INFO) > 0)
                            {
                                // Move image
                                UploadFile.MoveFile(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH] + ConfigurationManager.AppSettings[ConfigurationKeys.TEMP_USER_PATH] + "/" +
                                    file.FileName, ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH] + ConfigurationManager.AppSettings[ConfigurationKeys.USER_PATH] + "/" + GetLoginUser().CompanyCode + "/" + userId + "/" + ConfigurationManager.AppSettings[ConfigurationKeys.PROFILE_IMAGE] + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                            }
                        }

                        if (sessionLogin.Password != model.USER_INFO.password
                            && model.USER_INFO.password != Constant.DISPLAY_PASSWORD)
                        {
                            sessionLogin.Password = model.USER_INFO.password;
                            sessionLogin.Is_expired_password = false;
                        }

                        if (sessionLogin.DisplayName != model.USER_INFO.display_name)
                        {
                            sessionLogin.DisplayName = model.USER_INFO.display_name;
                        }

                        if (sessionLogin.ImageFilePath != model.USER_INFO.image_file_path)
                        {
                            sessionLogin.ImageFilePath = model.USER_INFO.image_file_path;
                        }

                        if (sessionLogin.ActualWorkInputMode != model.USER_INFO.actual_work_input_mode)
                        {
                            sessionLogin.ActualWorkInputMode = model.USER_INFO.actual_work_input_mode;
                        }
                        SetLoginUser(sessionLogin);

                        string action = model.USER_INFO.user_sys_id > 0 ? "更新" : "登録";
                        string message = string.Format(Resources.Messages.I007, "ユーザー情報", action);
                        model = MakePersonalSettingViewModel(userId);
                        JsonResult result = Json(
                                new
                                {
                                    statusCode = 201,
                                    message = message,
                                    id = model.USER_INFO.user_sys_id,
                                    row_version = Convert.ToBase64String(model.USER_INFO.row_version),
                                    imageFilePath = model.USER_INFO.image_file_path,
                                    userNameSesssion = sessionLogin.DisplayName
                                },
                                JsonRequestBehavior.AllowGet);
                        return result;
                    }
                    else
                    {
                        if (model.USER_INFO.user_sys_id > 0) // Duplicate action update
                        {
                            ViewBag.Duplicate = "/PMS01002/PersonalSetting";
                            string companyCode = GetLoginUser().CompanyCode;

                            model.GROUP_LIST = this.commonService.GetUserGroupSelectList(companyCode);
                            model.POSITION_LIST = this.GetPositionList(companyCode);
                            model.LANGUAGE_LIST = this.GetLanguageList();

                            return new EmptyResult();
                        }
                        else
                        {
                            ModelState.AddModelError("", Resources.Messages.E001);
                            return new EmptyResult();
                        }
                    }
                }

                ModelState.AddModelError("", Resources.Messages.E001);
                return new EmptyResult();
            }
            catch
            {
                JsonResult result = Json(
                    new
                    {
                        statusCode = 500,
                        message = string.Format(Resources.Messages.E045, "ユーザー情報")
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
        }

        /// <summary>
        /// User list
        /// </summary>
        /// <param name="callback">Callback function name</param>
        /// <returns>User list</returns>
        public ActionResult Select(int projectId = 0,
            string groupId = "",
            string pStart = "",
            string pTo = "",
            string isMultiSelect = "",
            string callback = "")
        {
            var currentUser = GetLoginUser();
            var model = new PMS01002ListViewModel
            {
                GROUP_LIST = this.commonService.GetUserGroupSelectList(currentUser.CompanyCode),
                POSITION_LIST = this.GetPositionList(currentUser.CompanyCode),
                IS_MULTI_SELECT = !string.IsNullOrEmpty(isMultiSelect) ? Constant.DEFAULT_VALUE : string.Empty,
                CallBack = callback
            };

            DateTime startTime;
            DateTime endTime;

            if (DateTime.TryParse(pStart, out startTime) && DateTime.TryParse(pTo, out endTime))
            {
                model.Condition.FROM_DATE = startTime.ToString("yyyy/MM/dd");
                model.Condition.TO_DATE = endTime.ToString("yyyy/MM/dd");
            }
            else
            {
                model.Condition.FROM_DATE = string.Empty;
                model.Condition.TO_DATE = string.Empty;
            }

            model.Condition.PROJECT_ID = projectId;

            if (!string.IsNullOrEmpty(groupId))
            {
                model.Condition.GROUP_ID = Convert.ToInt32(groupId);
            }

            return this.View("Select", model);
        }

        /// <summary>
        /// Get all position type by company code
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <returns>List of position type</returns>
        private IList<SelectListItem> GetPositionList(string cCode)
        {
            return
                this._service.GetPositionList(cCode)
                    .Select(
                        f =>
                        new SelectListItem
                        {
                            Value = f.position_id.ToString(),
                            Text = f.display_name
                        })
                    .ToList();
        }

        /// <summary>
        /// Get all role by company code
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <returns>List of role</returns>
        private IList<SelectListItem> GetAuthorityRoleList(string cCode)
        {
            return
                this._service.GetAuthorityRoleList(cCode)
                    .Select(
                        f =>
                        new SelectListItem
                        {
                            Value = f.role_id.ToString(),
                            Text = f.display_name
                        })
                    .ToList();
        }
        /// <summary>
        /// Get all branch list
        /// </summary>
        /// <returns></returns>
        private IList<SelectListItem> GetBranchList(string cCode)
        {
            return
                this._service.GetBranchList(cCode)
                    .Select(
                        f =>
                        new SelectListItem
                        {
                            Value = f.location_id.ToString(),
                            Text = f.display_name
                        }).ToList();
        }

        /// <summary>
        /// Get all language
        /// </summary>
        /// <returns>List of role</returns>
        private IList<SelectListItem> GetLanguageList()
        {
            return
                this._service.GetLanguageList()
                    .Select(
                        f =>
                        new SelectListItem
                        {
                            Value = f.language_id.ToString(),
                            Text = f.language
                        })
                    .ToList();
        }

        public ActionResult GetListUnitCost(int userId)
        {
            var currentUser = GetLoginUser();

            var data = this._service.GetListUnitCost(currentUser.CompanyCode, userId);

            JsonResult result = Json(from t in data
                                     select new object[] { t.apply_start_date != null ? t.apply_start_date.ToString("yyyy/MM") : "", t.base_unit_cost }.ToList(), JsonRequestBehavior.AllowGet);

            return result;
        }
    }
}
