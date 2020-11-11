using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10003;
using ProjectManagementSystem.ViewModels;
using ProjectManagementSystem.ViewModels.PMS10003;
using ProjectManagementSystem.WorkerServices;
using ProjectManagementSystem.WorkerServices.Impl;
using System.Data;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Controllers
{
    public class PMS10003Controller : ControllerBase
    {
        private readonly IPMS10003Service _service;

        private readonly IPMSCommonService _commonService;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS10003Controller()
            : this(new PMS10003Service(), new PMSCommonService())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS10003Controller(IPMS10003Service service, IPMSCommonService commonservice)
        {
            this._service = service;
            this._commonService = commonservice;
        }

        /// <summary>
        /// Company info
        /// </summary>
        /// <returns>Company Info View</returns>
        public ActionResult CompanyInfo()
        {
            if (!IsInFunctionList(Constant.FunctionID.CompanySetting))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            var model = MakeCompanyInfoViewModel();

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return this.View("CompanyInfo", model);
        }

        /// <summary>
        /// Make company info view.
        /// </summary>
        /// <returns>Company Info View Model</returns>
        private PMS10003CompanyInfoViewModel MakeCompanyInfoViewModel()
        {
            var model = new PMS10003CompanyInfoViewModel();
            model.PREFECTURE_LIST = this._commonService.GetPrefectureList();
            model.COMPANY_INFO = this._service.GetCompanyInfo(GetLoginUser().CompanyCode);
            return model;
        }

        /// <summary>
        /// Edit company info
        /// </summary>
        /// <param name="model">model</param>
        /// <returns>Edit Company info view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCompanyInfo(PMS10003CompanyInfoViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpPostedFileBase file = Request.Files["file"];
                    HttpPostedFileBase fileDrag = Request.Files["fileDrag"];

                    if (model.TypeUpload == "fileDrag" && fileDrag != null && fileDrag.FileName.Length > 0)
                    {
                        file = fileDrag;

                        string[] AllowedFileExtensions = new string[] { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };

                        if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                        {
                            model = MakeCompanyInfoViewModel();
                            ModelState.AddModelError("", String.Format(Resources.Messages.E010, "jpg,png.jpeg"));
                            return View("CompanyInfo", model);
                        }

                        if (file.ContentLength > Constant.MaxContentLength)
                        {
                            model = MakeCompanyInfoViewModel();
                            ModelState.AddModelError("", String.Format(Resources.Messages.E021, "500KB以内"));
                            return View("CompanyInfo", model);
                        }

                        model.COMPANY_INFO.logo_image_file_path = UploadFile.UploadFiles(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH], file, ConfigurationManager.AppSettings[ConfigurationKeys.COMPANY_PATH] + "/" + GetLoginUser().CompanyCode);
                    }
                    else if (model.TypeUpload == "file" && file != null && file.FileName.Length > 0)
                    {
                        string[] AllowedFileExtensions = new string[] { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };

                        if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                        {
                            model = MakeCompanyInfoViewModel();
                            ModelState.AddModelError("", String.Format(Resources.Messages.E010, "jpg,png.jpeg"));
                            return View("CompanyInfo", model);
                        }

                        if (file.ContentLength > Constant.MaxContentLength)
                        {
                            model = MakeCompanyInfoViewModel();
                            ModelState.AddModelError("", String.Format(Resources.Messages.E021, "500KB以内"));
                            return View("CompanyInfo", model);
                        }

                        model.COMPANY_INFO.logo_image_file_path = UploadFile.UploadFiles(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH], file, ConfigurationManager.AppSettings[ConfigurationKeys.COMPANY_PATH] + "/" + GetLoginUser().CompanyCode);
                    }
                    else
                    {
                        if (model.Clear == "1")
                        {
                            model.COMPANY_INFO.logo_image_file_path = string.Empty;
                        }
                    }

                    model.COMPANY_INFO.upd_date = Utility.GetCurrentDateTime();
                    model.COMPANY_INFO.upd_id = GetLoginUser().UserId;
                    model.COMPANY_INFO.company_code = GetLoginUser().CompanyCode;
                    model.COMPANY_INFO.zip_code = model.COMPANY_INFO.zip_code.Replace("-", "");
                    if (model.COMPANY_INFO.del_flg == null)
                    {
                        model.COMPANY_INFO.del_flg = Constant.DeleteFlag.NON_DELETE;
                    }

                    // Check duplicate email
                    if (!string.IsNullOrEmpty(model.COMPANY_INFO.mail_address))
                    {
                        if (_service.CheckCompanyEmail(model.COMPANY_INFO.mail_address, model.COMPANY_INFO.company_code) > 0)
                        {
                            model = MakeCompanyInfoViewModel();
                            ModelState.AddModelError("", String.Format(Resources.Messages.E008, "メールアドレス", "メールアドレス"));
                            return View("CompanyInfo", model);
                        }
                    }

                    if (_service.EditCompanyInfo(model.COMPANY_INFO))
                    {
                        var currentUser = GetLoginUser();

                        if (currentUser.CompanyLogoImgPath != model.COMPANY_INFO.logo_image_file_path)
                        {
                            currentUser.CompanyLogoImgPath = model.COMPANY_INFO.logo_image_file_path;
                        }

                        if (currentUser.CompanyName != model.COMPANY_INFO.display_name)
                        {
                            currentUser.CompanyName = model.COMPANY_INFO.display_name;
                        }

                        SetLoginUser(currentUser);
                        string message = string.Format(Resources.Messages.I007, "会社情報", "更新");

                        var data = this._service.GetCompanyInfo(model.COMPANY_INFO.company_code);

                        JsonResult result = Json(
                                new
                                {
                                    statusCode = 201,
                                    message = message,
                                    id = model.COMPANY_INFO.company_code,
                                    insDate = (data.ins_date != null) ? data.ins_date.ToString("yyyy/MM/dd HH:mm") : "",
                                    updDate = (data.upd_date != null) ? data.upd_date.ToString("yyyy/MM/dd HH:mm") : "",
                                    insUser = data.user_regist,
                                    updUser = data.user_update,
                                    logoImageFilePath = data.logo_image_file_path,
                                    companyNameSesssion = currentUser.CompanyName
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
                                        message = string.Format(Resources.Messages.E045, "会社情報")
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
                         message = string.Format(Resources.Messages.E045, "会社情報")
                     },
                     JsonRequestBehavior.AllowGet);

                return result;
            }
        }

        /// <summary>
        /// Company Detail
        /// </summary>
        /// <returns>Company Detail View</returns>
        public ActionResult CompanyDetail()
        {
            if (!IsInFunctionList(Constant.FunctionID.CompanySetting))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            var model = MakeCompanyDetailViewModel();

            return this.View("CompanyDetail", model);
        }

        /// <summary>
        /// Make company Detail View
        /// </summary>
        /// <returns>Company Detail View Model</returns>
        private PMS10003CompanyDetailViewModel MakeCompanyDetailViewModel()
        {
            var model = new PMS10003CompanyDetailViewModel();
            model.COMPANY_SETTING = this._service.GetCompanySetting(GetLoginUser().CompanyCode);
            model.SPECIAL_HOLIDAY_LIST = this._service.GetListSpecialHoliday(GetLoginUser().CompanyCode);
            List<int> listWeeklyHoliday = new List<int>();

            if (model.COMPANY_SETTING.default_holiday_type != null)
            {
                listWeeklyHoliday = model.COMPANY_SETTING.default_holiday_type.Split(',').Select(int.Parse).ToList();
            }

            for (int i = 0; i < 7; i++)
            {
                if (listWeeklyHoliday.Contains(i))
                {
                    model.HOLIDAY_TYPE_LIST.Add(new Weekday(true));
                }
                else
                {
                    model.HOLIDAY_TYPE_LIST.Add(new Weekday(false));
                }
            }

            int checkPointWeek = 0;

            if (!string.IsNullOrEmpty(model.COMPANY_SETTING.check_point_week))
            {
                checkPointWeek = int.Parse(model.COMPANY_SETTING.check_point_week);
            }

            for (int i = 0; i < 7; i++)
            {
                if (checkPointWeek == i)
                {
                    model.CHECK_POINT_WEEK_LIST.Add(new WeekPointDay(true));
                }
                else
                {
                    model.CHECK_POINT_WEEK_LIST.Add(new WeekPointDay(false));
                }
            }
            return model;
        }

        /// <summary>
        /// Class Year Month
        /// </summary>
        private class YearMonth
        {
            public int Year { get; set; }
            public int Month { get; set; }
        }

        /// <summary>
        /// Check if there is no working day
        /// </summary>
        /// <param name="ym">ym</param>
        /// <param name="defaultHoliday">defaultHoliday</param>
        /// <param name="specialHoliday">specialHoliday</param>
        /// <returns>bool: true/fale</returns>
        private bool isNonOfWorkingDay(YearMonth ym, List<int> defaultHoliday, List<DateTime> specialHoliday)
        {

            var numOfdays = DateTime.DaysInMonth(ym.Year, ym.Month);
            for (var i = 1; i <= numOfdays; i++)
            {
                var date = new DateTime(ym.Year, ym.Month, i);
                if (!defaultHoliday.Contains((int)date.DayOfWeek)
                    && !specialHoliday.Contains(date))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Edit Company Company Detail
        /// </summary>
        /// <param name="model">model</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCompanyDetail(PMS10003CompanyDetailViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int checkPointWeek = 0;

                    for (int i = 0; i < 7; i++)
                    {
                        if (model.CHECK_POINT_WEEK_LIST.ElementAt(i).isCheckPointWeek)
                        {
                            checkPointWeek = i;
                        }
                    }

                    
                    List<int> listHolidayInWeek = new List<int>();

                    for (int i = 0; i < 7; i++)
                    {
                        if (model.HOLIDAY_TYPE_LIST.ElementAt(i).isHoliday)
                        {
                            listHolidayInWeek.Add(i);
                        }
                    }

                    // check default holiday type
                    if (listHolidayInWeek.Count == 7)
                    {
                        ModelState.AddModelError("", Messages.E061);
                        ViewBag.ErrorClass = "label-validation-error";
                        return View("CompanyDetail", model);
                    }
                    else
                    {
                        // create list of special holiday date
                        List<DateTime> listSpecialHolidayDate = new List<DateTime>();
                        foreach (var holiday in model.SPECIAL_HOLIDAY_LIST)
                        {
                            if (holiday.holiday_date != null)
                            {
                                listSpecialHolidayDate.Add((DateTime)holiday.holiday_date);
                            }
                        }
                        // create list of special holiday year-month
                        List<YearMonth> listSpecialYearMonth = new List<YearMonth>();
                        foreach (var holiday_date in listSpecialHolidayDate)
                        {
                            YearMonth ym = new YearMonth();
                            ym.Year = holiday_date.Year;
                            ym.Month = holiday_date.Month;
                            // Check if this month has been checked
                            if (!listSpecialYearMonth.Contains(ym))
                            {
                                // check if thif month have non of working time
                                if (isNonOfWorkingDay(ym, listHolidayInWeek, listSpecialHolidayDate))
                                {
                                    ModelState.AddModelError("", Messages.E062);
                                    ViewBag.ErrorClass = "label-validation-error";
                                    ViewBag.ErrorClassNonWorking = "label-validation-error";
                                    return View("CompanyDetail", model);
                                }
                                listSpecialYearMonth.Add(ym);
                            }
                        }
                    }

                    var DateTimeNow = Utility.GetCurrentDateTime();
                    var currentUserId = GetLoginUser().UserId;

                    model.COMPANY_SETTING.default_holiday_type = string.Join(",", listHolidayInWeek);
                    model.COMPANY_SETTING.upd_date = DateTimeNow;
                    model.COMPANY_SETTING.upd_id = currentUserId;
                    model.COMPANY_SETTING.company_code = GetLoginUser().CompanyCode;
                    model.COMPANY_SETTING.check_point_week = checkPointWeek.ToString();
                    foreach (var holiday in model.SPECIAL_HOLIDAY_LIST)
                    {
                        holiday.ins_id = currentUserId;
                        holiday.ins_date = DateTimeNow;
                    }

                    if (_service.EditCompanyDetail(model.COMPANY_SETTING, model.SPECIAL_HOLIDAY_LIST))
                    {
                        var currentUser = GetLoginUser();
                        if (currentUser.DecimalCalculationType != model.COMPANY_SETTING.decimal_calculation_type)
                        {
                            currentUser.DecimalCalculationType = model.COMPANY_SETTING.decimal_calculation_type;
                        }
                        if (currentUser.Working_time_unit_minute != model.COMPANY_SETTING.working_time_unit_minute)
                        {
                            currentUser.Working_time_unit_minute = model.COMPANY_SETTING.working_time_unit_minute;
                        }
                        SetLoginUser(currentUser);
                        string action = !string.IsNullOrEmpty(model.COMPANY_SETTING.company_code) ? "更新" : "登録";
                        string message = string.Format(Resources.Messages.I007, "会社情報詳細", action);

                        var data = this._service.GetCompanySetting(model.COMPANY_SETTING.company_code);

                        JsonResult result = Json(
                                new
                                {
                                    statusCode = 201,
                                    message = message,
                                    id = model.COMPANY_SETTING.company_code,
                                    insDate = (data.ins_date != null) ? data.ins_date.ToString("yyyy/MM/dd HH:mm") : "",
                                    updDate = (data.upd_date != null) ? data.upd_date.ToString("yyyy/MM/dd HH:mm") : "",
                                    insUser = data.user_regist,
                                    updUser = data.user_update,
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
                                        message = string.Format(Resources.Messages.E045, "フェーズ情報")
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
                         message = string.Format(Resources.Messages.E045, "会社情報詳細")
                     },
                     JsonRequestBehavior.AllowGet);

                return result;
            }
        }
    }
}
