using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS02001;
using ProjectManagementSystem.ViewModels;
using ProjectManagementSystem.ViewModels.PMS02001;
using ProjectManagementSystem.WorkerServices;
using ProjectManagementSystem.WorkerServices.Impl;

namespace ProjectManagementSystem.Controllers
{
    using System.Data;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.Text;

    public class PMS02001Controller : ControllerBase
    {
        private readonly IPMS02001Service _service;
        private readonly IPMSCommonService commonService;

        /// <summary>
        /// TempData storage
        /// </summary>
        [System.Serializable]
        private class TmpValues
        {
            public int CustomerID { get; set; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS02001Controller(IPMS02001Service service, IPMSCommonService cmService)
        {
            this._service = service;
            this.commonService = cmService;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS02001Controller()
            : this(new PMS02001Service(), new PMSCommonService())
        {
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>List View</returns>
        public ActionResult Index()
        {
            if ((!IsInFunctionList(Constant.FunctionID.CustomerList_Admin)) && (!IsInFunctionList(Constant.FunctionID.CustomerList)))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            var model = new PMS02001ListViewModel();

            // Get Jquery data table state
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS02001/Edit") && Session[Constant.SESSION_IS_BACK] != null)
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
        /// Function input zip code
        /// </summary>
        /// <param name="zipcode">zipcode</param>
        /// <returns>Json Zip Code</returns>
        [HttpPost]
        public JsonResult ImportZipCode(string zipcode)
        {
            try
            {
                string fileName = "KEN_ALL.CSV";
                string filePath = ConfigurationManager.AppSettings[ConfigurationKeys.APP_DATA_FILE];
                if (!Directory.Exists(Server.MapPath(filePath)))
                {
                    Directory.CreateDirectory(Server.MapPath(filePath));
                }

                string path = Path.Combine(Server.MapPath(filePath), fileName);
                DataTable dt = ProcessCSV(path, zipcode);
                Dictionary<String, Object> obj = new Dictionary<String, Object>();

                if(dt.Rows.Count > 0){
                    obj.Add("prefectures", dt.Rows[0][6].ToString().Replace("\"", ""));
                    obj.Add("city", dt.Rows[0][7].ToString().Replace("\"", ""));
                    obj.Add("address", dt.Rows[0][8].ToString().Replace("\"", ""));
                }

                if (obj.Count > 0)
                {
                    return Json(obj);
                }
                else
                {
                    return Json(string.Empty);
                }
            }
            catch
            {
                return Json(string.Empty);
            }
        }

        /// <summary>
        /// Functon Process CSV 
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// <param name="zipcode">zipcode</param>
        /// <returns>DataTable</returns>
        private static DataTable ProcessCSV(string fileName, string zipcode)
        {
            //Set up our variables
            string Feedback = string.Empty;
            string line = string.Empty;
            DataTable dt = new DataTable();
            DataRow row;
            // work out where we should split on comma, but not in a sentence
            Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            //Set the filename in to our stream
            StreamReader sr = new StreamReader(fileName, Encoding.GetEncoding("Shift-JIS"), true);
            try
            {
                for (int i = 1; i <= 15; i++ )
                {
                    dt.Columns.Add(new DataColumn(i.ToString()));
                }

                //Read each line in the CVS file until it’s empty
                while ((line = sr.ReadLine()) != null)
                {
                    row = dt.NewRow();

                    //add our current value to our data row
                    string[] tmpArr = r.Split(line);
                    if (tmpArr[2].ToString().Replace("\"", "").Equals(zipcode))
                    {
                        row.ItemArray = tmpArr;
                        dt.Rows.Add(row);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //Tidy Streameader up
                sr.Dispose();
            }

            //return a the new DataTable
            return dt;
        }

        /// <summary>
        /// Search customer by condition
        /// </summary>
        /// <param name="model">dataTable info</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Json list information</returns>
        public ActionResult Search(DataTablesModel model, Condition condition, string orderBy, string orderType, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid)
                {
                    var pageInfo = this._service.Search(model, condition, GetLoginUser().CompanyCode);

                    if (!String.IsNullOrEmpty(TAB_ID)) // if current screen is Customer List
                    {
                        Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = this._service.GetListCustomer(condition, GetLoginUser().CompanyCode, orderBy, orderType);
                    }

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
                                                      t.customer_id,
                                                      t.peta_rn,
                                                      HttpUtility.HtmlEncode(t.display_name), 
                                                      HttpUtility.HtmlEncode(t.customer_name_kana), 
                                                      HttpUtility.HtmlEncode(t.address),
                                                      (t.url != null)? HttpUtility.HtmlEncode(t.url) : "", 
                                                      (t.upd_date != null) ? t.upd_date.ToString("yyyy/MM/dd HH:mm") : "",
                                                      (t.user_update != null)? HttpUtility.HtmlEncode(t.user_update) : "",
                                                      t.del_flg
                                                  }).ToList()
                        });
                    SaveRestoreData(condition);
                    return result;
                }
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Export to csv customer list
        /// </summary>
        /// <param name="search_displayName"></param>
        /// <param name="search_customerNameKata"></param>
        /// <param name="search_deleteFlag"></param>
        /// <param name="hdnOrderBy"></param>
        /// <param name="hdnOrderType"></param>
        /// <returns></returns>
        public ActionResult ExportCsvListCustomer(string search_displayName, string search_customerNameKata, bool search_deleteFlag, string hdnOrderBy, string hdnOrderType, string TAB_ID)
        {
            Condition condition = new Condition();
            condition.DISPLAY_NAME = search_displayName;
            condition.CUSTOMER_NAME_KATA = search_customerNameKata;
            condition.DELETED_INCLUDE = search_deleteFlag;

            if (string.IsNullOrEmpty(hdnOrderBy))
                hdnOrderBy = "upd_date";

            IList<CustomerPlus> results = new List<CustomerPlus>();
            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                results = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<CustomerPlus>;
            }
            else
            {
                results = this._service.GetListCustomer(condition, GetLoginUser().CompanyCode, hdnOrderBy, hdnOrderType);
            } 

            List<CustomerPlusExport> dataExport = new List<CustomerPlusExport>();
            string[] columns = new[] {
                    "No.",
                    "取引先名",
                    "取引先名（カナ）",
                    "住所",
                    "URL",
                    "更新日時",
                    "更新者"
            };
            for (int i = 0; i < results.Count; i++)
            {
                results[i].peta_rn = i + 1;
            }

            foreach (var r in results)
            {
                CustomerPlusExport tmpData = new CustomerPlusExport();
                tmpData.customer_id = r.peta_rn;
                tmpData.display_name = r.display_name;
                tmpData.customer_name_kana = r.customer_name_kana;
                tmpData.address = r.address;
                tmpData.url = (r.url != null)? r.url : "";
                tmpData.upd_date = (r.upd_date != null) ? r.upd_date.ToString("yyyy/MM/dd HH:mm") : "";
                tmpData.user_update = (r.user_update != null) ? r.user_update : "";
                dataExport.Add(tmpData);
            }

            DataTable dt = Utility.ToDataTableT(dataExport, columns.ToArray());
            string fileName = "CustomerList_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";
            Utility.ExportToCsvData(this, dt, fileName);

            return new EmptyResult();
        }

        /// <summary>
        /// Function process file CSV to Datatable
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// <returns>DataTable</returns>
        private static DataTable ProcessCSV(string fileName)
        {
            //Set up our variables
            string Feedback = string.Empty;
            string line = string.Empty;
            string[] strArray;
            DataTable dt = new DataTable();
            DataRow row;
            // work out where we should split on comma, but not in a sentence
            Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            //Set the filename in to our stream
            StreamReader sr = new StreamReader(fileName, Encoding.GetEncoding("shift-jis"), true);
            try
            {
                //Read the first line and split the string at , with our regular expression in to an array
                line = sr.ReadLine();
                strArray = r.Split(line);

                //For each item in the new split array, dynamically builds our Data columns. Save us having to worry about it.
                Array.ForEach(strArray, s => dt.Columns.Add(new DataColumn(s)));

                //Read each line in the CVS file until it’s empty
                while ((line = sr.ReadLine()) != null)
                {
                    row = dt.NewRow();

                    //add our current value to our data row
                    string[] tmpArr = r.Split(line);

                    if (tmpArr.Length > 14)
                    {
                        throw new Exception();
                    }

                    for (int i = 0; i < tmpArr.Length; i++)
                    {
                        string newData = string.IsNullOrEmpty(tmpArr[i]) ? "" : tmpArr[i].Replace("\"", "");

                        tmpArr[i] = newData;
                    }

                    row.ItemArray = tmpArr;
                    dt.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //Tidy Streameader up
                sr.Dispose();
            }

            //return a the new DataTable
            return dt;

        }

        /// <summary>
        /// Import data customer from csv file
        /// </summary>
        /// <param name="csvFile">csv file</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImportCustomerCsv(HttpPostedFileBase csvFile)
        {
            // check file import
            if (csvFile.FileName.Length == 0 || csvFile.ContentLength == 0)
            {
                JsonResult errFile = Json(
                    new
                    {
                        statusCode = Constant.HttpResponseCode.EXPECTATION_FAILED
                    },
                    JsonRequestBehavior.AllowGet);

                return errFile;
            }

            // save file to read
            HttpPostedFileBase file = Request.Files[0];
            string fileName = Path.GetFileName(file.FileName);
            string filePath = ConfigurationManager.AppSettings[ConfigurationKeys.APP_DATA_FILE];

            if (!Directory.Exists(Server.MapPath(filePath)))
            {
                Directory.CreateDirectory(Server.MapPath(filePath));
            }

            string path = Path.Combine(Server.MapPath(filePath), fileName);
            file.SaveAs(path);

            // read file
            DataTable dt = new DataTable();

            try
            {
                dt = ProcessCSV(path);
            }
            catch
            {
                JsonResult errFormat = Json(
                    new
                    {
                        statusCode = Constant.HttpResponseCode.EXPECTATION_FAILED
                    },
                    JsonRequestBehavior.AllowGet);

                return errFormat;
            }
            finally
            {
                // delete file after read
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            try
            {
                var loginUser = GetLoginUser();

                // check max data can insert
                if (!this.commonService.CheckValidUpdateData(loginUser.CompanyCode, Constant.LicenseDataType.CUSTOMER, dt.Rows.Count))
                {
                    JsonResult errMaxData = Json(
                        new
                        {
                            statusCode = Constant.HttpResponseCode.INTERNAL_SERVER_ERROR,
                            message = string.Format(Resources.Messages.E067, "取引先")
                        },
                        JsonRequestBehavior.AllowGet);

                    return errMaxData;
                }

                IList<int> errRowList = new List<int>();
                IList<Customer> customerList = new List<Customer>();
                IList<string> emailList = new List<string>();
                var prefectureList = this._service.GetPrefectureList();
                int indexRow = 0;
                bool validRegister = true;

                // read data from csv file
                foreach (DataRow dtRow in dt.Rows)
                {
                    indexRow++;
                    bool invalidRow = false;

                    // check required data
                    if (dtRow[Constant.CustomerCsvImportKey.display_name].ToString() == "")
                    {
                        invalidRow = true;
                    }

                    // check length data
                    if (dtRow[Constant.CustomerCsvImportKey.customer_name].ToString().Length > Constant.MAX_LENGTH_NAME
                        || dtRow[Constant.CustomerCsvImportKey.customer_name_kana].ToString().Length > Constant.MAX_LENGTH_NAME
                        || dtRow[Constant.CustomerCsvImportKey.display_name].ToString().Length > Constant.MAX_LENGTH_NAME
                        || dtRow[Constant.CustomerCsvImportKey.city].ToString().Length > Constant.MAX_LENGTH_ADDRESS
                        || dtRow[Constant.CustomerCsvImportKey.address_1].ToString().Length > Constant.MAX_LENGTH_ADDRESS
                        || dtRow[Constant.CustomerCsvImportKey.address_2].ToString().Length > Constant.MAX_LENGTH_ADDRESS
                        || dtRow[Constant.CustomerCsvImportKey.tel_no].ToString().Length > Constant.MAX_LENGTH_TEL_FAX_NO
                        || dtRow[Constant.CustomerCsvImportKey.fax_no].ToString().Length > Constant.MAX_LENGTH_TEL_FAX_NO
                        || dtRow[Constant.CustomerCsvImportKey.url].ToString().Length > Constant.MAX_LENGTH_ADDRESS
                        || dtRow[Constant.CustomerCsvImportKey.remarks].ToString().Length > Constant.MAX_LENGTH_REMARK)
                    {
                        invalidRow = true;
                    }

                    string customerNameEn = dtRow[Constant.CustomerCsvImportKey.customer_name_en].ToString();

                    // check length & format data
                    if (!string.IsNullOrEmpty(customerNameEn))
                    {
                        if (customerNameEn.Length > Constant.MAX_LENGTH_NAME)
                        {
                            invalidRow = true;
                        }

                        if (!Utility.ChecFullHalfSizeValid(customerNameEn))
                        {
                            invalidRow = true;
                        }
                    }

                    string zipCode = dtRow[Constant.CustomerCsvImportKey.zip_code].ToString();

                    // check length & format data
                    if (!string.IsNullOrEmpty(zipCode))
                    {
                        zipCode = zipCode.Replace("-", "");

                        if (zipCode.Length > 7)
                        {
                            invalidRow = true;
                        }

                        if (!Utility.CheckPhoneNumberValid(dtRow[Constant.CustomerCsvImportKey.zip_code].ToString()))
                        {
                            invalidRow = true;
                        }
                    }

                    // check format data
                    if (!string.IsNullOrEmpty(dtRow[Constant.CustomerCsvImportKey.tel_no].ToString()) && !Utility.CheckPhoneNumberValid(dtRow[Constant.CustomerCsvImportKey.tel_no].ToString()))
                    {
                        invalidRow = true;
                    }

                    // check format data
                    if (!string.IsNullOrEmpty(dtRow[Constant.CustomerCsvImportKey.fax_no].ToString()) && !Utility.CheckPhoneNumberValid(dtRow[Constant.CustomerCsvImportKey.fax_no].ToString()))
                    {
                        invalidRow = true;
                    }

                    string mailAddress = dtRow[Constant.CustomerCsvImportKey.mail_address].ToString();

                    if (!string.IsNullOrEmpty(mailAddress))
                    {
                        // check length
                        if (mailAddress.Length > Constant.MAX_LENGTH_ADDRESS)
                        {
                            invalidRow = true;
                        }

                        // check format data
                        if (!Utility.CheckEmailValid(mailAddress))
                        {
                            invalidRow = true;
                        }

                        // check exist email
                        if (_service.CheckCustomerEmail(mailAddress, 0) > 0)
                        {
                            invalidRow = true;
                        }

                        emailList.Add(mailAddress);
                    }

                    // check format data
                    if (!string.IsNullOrEmpty(dtRow[Constant.CustomerCsvImportKey.url].ToString()) && !Utility.CheckURLValid(dtRow[Constant.CustomerCsvImportKey.url].ToString()))
                    {
                        invalidRow = true;
                    }

                    if (invalidRow)
                    {
                        validRegister = false;
                        errRowList.Add(indexRow);
                    }
                    else
                    {
                        Customer customerEntity = new Customer();

                        customerEntity.upd_date = Utility.GetCurrentDateTime();
                        customerEntity.upd_id = loginUser.UserId;
                        customerEntity.del_flg = Constant.DeleteFlag.NON_DELETE;
                        customerEntity.company_code = loginUser.CompanyCode;

                        customerEntity.customer_name = dtRow[Constant.CustomerCsvImportKey.customer_name].ToString();
                        customerEntity.customer_name_kana = dtRow[Constant.CustomerCsvImportKey.customer_name_kana].ToString();
                        customerEntity.customer_name_en = dtRow[Constant.CustomerCsvImportKey.customer_name_en].ToString();
                        customerEntity.display_name = dtRow[Constant.CustomerCsvImportKey.display_name].ToString();
                        customerEntity.zip_code = zipCode;
                        customerEntity.prefecture_code = this.GetPrefectureCode(prefectureList, dtRow[Constant.CustomerCsvImportKey.prefecture_name].ToString());
                        customerEntity.city = dtRow[Constant.CustomerCsvImportKey.city].ToString();
                        customerEntity.address_1 = dtRow[Constant.CustomerCsvImportKey.address_1].ToString();
                        customerEntity.address_2 = dtRow[Constant.CustomerCsvImportKey.address_2].ToString();
                        customerEntity.tel_no = dtRow[Constant.CustomerCsvImportKey.tel_no].ToString();
                        customerEntity.fax_no = dtRow[Constant.CustomerCsvImportKey.fax_no].ToString();
                        customerEntity.mail_address = dtRow[Constant.CustomerCsvImportKey.mail_address].ToString();
                        customerEntity.url = dtRow[Constant.CustomerCsvImportKey.url].ToString();
                        customerEntity.remarks = dtRow[Constant.CustomerCsvImportKey.remarks].ToString();

                        customerList.Add(customerEntity);
                    }
                }

                // data is valid
                if (validRegister)
                {
                    // get duplicate email in csv
                    var duplicateKeys = emailList.GroupBy(m => m).Where(group => group.Count() > 1).Select(group => group.Key);

                    // check duplicate email
                    if (duplicateKeys.Count() > 0)
                    {
                        JsonResult errDuplicateEmail = Json(
                        new
                        {
                            statusCode = Constant.HttpResponseCode.INTERNAL_SERVER_ERROR,
                            message = "メールアドレスは重複登録できません。"
                        },
                        JsonRequestBehavior.AllowGet);

                        return errDuplicateEmail;
                    }
                    else if (_service.ImportCustomerData(customerList) == 0) // insert data to DB
                    {
                        validRegister = false;
                    }
                }

                int statusResponse = validRegister ? Constant.HttpResponseCode.SUCCESSFUL : Constant.HttpResponseCode.EXPECTATION_FAILED;

                JsonResult result = Json(
                    new
                    {
                        statusCode = statusResponse,
                        errRowList = errRowList
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
            catch
            {
                JsonResult result = Json(
                    new
                    {
                        statusCode = Constant.HttpResponseCode.EXPECTATION_FAILED,
                        validRegister = false
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
        }

        /// <summary>
        /// Get prefecture code from master data
        /// </summary>
        /// <param name="prefectureList"></param>
        /// <param name="prefectureName"></param>
        /// <returns></returns>
        private string GetPrefectureCode(IEnumerable<Prefecture> prefectureList, string prefectureName)
        {
            string prefectureCode = "";

            if (!string.IsNullOrEmpty(prefectureName))
            {
                foreach (var item in prefectureList)
                {
                    if (prefectureName.Equals(item.prefecture_name))
                    {
                        prefectureCode = item.prefecture_code;
                        break;
                    }
                }
            }

            return prefectureCode;
        }


        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        public ActionResult Edit(int id = 0)
        {
            if (!IsInFunctionList(Constant.FunctionID.CustomerRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            var model = MakeEditViewModel(id);

            // Keep key id

            return this.View("Edit", model);
        }

        /// <summary>
        /// Edit infomation Customer
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Edit view</returns>
        [HttpGet]
        public ActionResult Edit()
        {
            if (!IsInFunctionList(Constant.FunctionID.CustomerRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            var model = MakeEditViewModel(0);

            // Keep key id

            return this.View("Edit", model);
        }

        /// <summary>
        /// Create object PMS02001EditViewModel
        /// </summary>
        /// <param name="customerId">customerId</param>
        /// <returns>Edit View Model</returns>
        private PMS02001EditViewModel MakeEditViewModel(int customerId)
        {
            var model = new PMS02001EditViewModel();
            model.PREFECTURE_LIST = this.GetPrefectureList();
            if (customerId > 0)
            {
                model.CUSTOMER_INFO = this._service.GetCustomerInfo(GetLoginUser().CompanyCode, customerId);
                model.CUSTOMER_INFO.user_regist = model.CUSTOMER_INFO.user_regist;
                model.CUSTOMER_INFO.user_update = model.CUSTOMER_INFO.user_update;
            }

            return model;
        }

        /// <summary>
        /// Edit information Customer [POST]
        /// </summary>
        /// <param name="model">PMS02001EditViewModel</param>
        /// <returns>Edit View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomer(PMS02001EditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpPostedFileBase file = Request.Files["file"];
                    HttpPostedFileBase fileDrag = Request.Files["fileDrag"];
                    if (model.TypeUpload == "file" && file != null && file.FileName.Length > 0)
                    {
                        if (! Constant.AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                        {
                            ModelState.AddModelError("", String.Format(Resources.Messages.E010, "jpg,png.jpeg"));
                            return new EmptyResult();
                        }

                        if (file.ContentLength > Constant.MaxContentLength)
                        {
                            ModelState.AddModelError("", String.Format(Resources.Messages.E021, "500KB以内"));
                            return new EmptyResult();
                        }

                        model.CUSTOMER_INFO.logo_image_file_path = UploadFile.UploadFiles(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH], file, ConfigurationManager.AppSettings[ConfigurationKeys.TEMP_CUSTOMER_PATH]);
                    }
                    else if (model.TypeUpload == "fileDrag" && fileDrag != null && fileDrag.FileName.Length > 0)
                    {
                        file = fileDrag;

                        if (!Constant.AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                        {
                            ModelState.AddModelError("", String.Format(Resources.Messages.E010, "jpg,png.jpeg"));
                            return new EmptyResult();
                        }

                        if (file.ContentLength > Constant.MaxContentLength)
                        {
                            ModelState.AddModelError("", String.Format(Resources.Messages.E021, "500KB以内"));
                            return new EmptyResult();
                        }

                        model.CUSTOMER_INFO.logo_image_file_path = UploadFile.UploadFiles(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH], file, ConfigurationManager.AppSettings[ConfigurationKeys.TEMP_CUSTOMER_PATH]);
                    }
                    else
                    {
                        if (model.Clear == "1")
                        {
                            model.CUSTOMER_INFO.logo_image_file_path = string.Empty;
                        }
                    }

                    var loginUser = GetLoginUser();

                    model.CUSTOMER_INFO.upd_date = Utility.GetCurrentDateTime();
                    model.CUSTOMER_INFO.upd_id = loginUser.UserId;
                    model.CUSTOMER_INFO.company_code = loginUser.CompanyCode;
                    model.CUSTOMER_INFO.zip_code = model.CUSTOMER_INFO.zip_code.Replace("-", "");

                    if (!string.IsNullOrEmpty(model.CUSTOMER_INFO.mail_address))
                    {
                        if (_service.CheckCustomerEmail(model.CUSTOMER_INFO.mail_address, model.CUSTOMER_INFO.customer_id) > 0)
                        {
                            ModelState.AddModelError("", String.Format(Resources.Messages.E008, "メールアドレス", "メールアドレス"));
                            return new EmptyResult();
                        }
                    }

                    if ((model.CUSTOMER_INFO.customer_id == 0
                        || (model.OLD_DEL_FLAG
                        && Constant.DeleteFlag.NON_DELETE.Equals(model.CUSTOMER_INFO.del_flg)))
                        && !this.commonService.CheckValidUpdateData(loginUser.CompanyCode, Constant.LicenseDataType.CUSTOMER))
                    {
                        JsonResult result = Json(
                            new
                            {
                                statusCode = 500,
                                message = string.Format(Resources.Messages.E067, "取引先")
                            },
                            JsonRequestBehavior.AllowGet);

                        return result;
                    }

                    int tempcustomerID = model.CUSTOMER_INFO.customer_id;
                    int customerID = _service.EditCustomerData(model.CUSTOMER_INFO);
                    if (customerID > 0)
                    {
                        if (file != null && file.FileName.Length > 0)
                        {
                            UploadFile.CreateFolder(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH] + ConfigurationManager.AppSettings[ConfigurationKeys.CUSTOMER_PATH] + "/" + loginUser.CompanyCode + "/" + customerID.ToString());
                            model.CUSTOMER_INFO.logo_image_file_path = ConfigurationManager.AppSettings[ConfigurationKeys.CUSTOMER_PATH] + "/" + loginUser.CompanyCode + "/" + customerID + "/" + ConfigurationManager.AppSettings[ConfigurationKeys.LOGO_IMAGE] + file.FileName.Substring(file.FileName.LastIndexOf('.'));
                            model.CUSTOMER_INFO.row_version = this._service.GetCustomerInfo(loginUser.CompanyCode, customerID).row_version;
                            model.CUSTOMER_INFO.customer_id = customerID;

                            if (model.CUSTOMER_INFO.del_flg == null)
                            {
                                model.CUSTOMER_INFO.del_flg = Constant.DeleteFlag.NON_DELETE;
                            }

                            if (_service.EditCustomerData(model.CUSTOMER_INFO) > 0)
                            {
                                // Move image
                                UploadFile.MoveFile(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH] + ConfigurationManager.AppSettings[ConfigurationKeys.TEMP_CUSTOMER_PATH] + "/" +
                                    file.FileName, ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH] + ConfigurationManager.AppSettings[ConfigurationKeys.CUSTOMER_PATH] + "/" + loginUser.CompanyCode + "/" + customerID + "/" + ConfigurationManager.AppSettings[ConfigurationKeys.LOGO_IMAGE] + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                            }
                        }

                        string action = Convert.ToInt32(tempcustomerID) > 0 ? "更新" : "登録";
                        //string action = model.CUSTOMER_INFO.row_version.Length > 0 ? "更新" : "登録";
                        string message = String.Format(Resources.Messages.I007, "取引先情報", action);

                        var data = this._service.GetCustomerInfo(loginUser.CompanyCode, customerID);

                        JsonResult result = Json(
                                new
                                {
                                    statusCode = 201,
                                    message = message,
                                    id = customerID,
                                    row_version = Convert.ToBase64String(data.row_version),
                                    insDate = (data.ins_date != null) ? data.ins_date.ToString("yyyy/MM/dd HH:mm") : "",
                                    updDate = (data.upd_date != null) ? data.upd_date.ToString("yyyy/MM/dd HH:mm") : "",
                                    insUser = data.user_regist,
                                    updUser = data.user_update,
                                    deleted = data.del_flg.Equals(Constant.DeleteFlag.DELETE) ? true : false,
                                    logoImageFilePath = data.logo_image_file_path
                                },
                                JsonRequestBehavior.AllowGet);

                        return result;
                    }
                    else
                    {
                        if (model.CUSTOMER_INFO.row_version.Length > 0) // Duplicate action update
                        {
                            JsonResult result = Json(
                                new
                                {
                                    statusCode = 202,
                                    message = string.Format(Resources.Messages.E031)
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
                                    message = string.Format(Resources.Messages.E045, "取引先情報")
                                },
                                JsonRequestBehavior.AllowGet);

                            return result;
                        }
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
                        message = string.Format(Resources.Messages.E045, "取引先情報")
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
        }

        /// <summary>
        /// Customer list
        /// </summary>
        /// <param name="callback">Callback function name</param>
        /// <returns>Customer list</returns>
        public ActionResult Select(string isMultiSelect = "", string callback = "" , int projectId = 0)
        {
            var model = new PMS02001ListViewModel {
                IS_MULTI_SELECT = !string.IsNullOrEmpty(isMultiSelect) ? Constant.DEFAULT_VALUE : string.Empty,
                CallBack = callback,
                SearchByObject = (callback == "setOutsourcer") ? SearchObject.Customer : SearchObject.EndUser
            };

            model.Condition.PROJECT_ID = projectId;

            return this.View("Select", model);
        }

        /// <summary>
        /// Function Get Perfecture List
        /// </summary>
        /// <returns>List Prefecture</returns>
        private IList<SelectListItem> GetPrefectureList()
        {
            return
                this._service.GetPrefectureList()
                    .Select(
                        f =>
                        new SelectListItem
                        {
                            Value = f.prefecture_code.ToString(),
                            Text = f.prefecture_name
                        })
                    .ToList();
        }
    }
}
