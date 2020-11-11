namespace ProjectManagementSystem.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using AdvanceSoftware.ExcelCreator.Xlsx;
    using ProjectManagementSystem.Models.PMS06001;
    using ProjectManagementSystem.Models.PMS06002;
    using System.Net.Mail;
    using System.IO;
    using System.Drawing;
    using ViewModels.PMS11003;
    using Models.PMS11003;
    using System.Globalization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Dynamic;
    using Newtonsoft.Json;

    public class Utility
    {
        #region Format data
        /// <summary>
        /// Used to format string to display
        /// </summary>
        /// <param name="name"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static string Shorten(string name, int chars = 10)
        {
            if (string.IsNullOrEmpty(name))
                return name;
            if (name.ToCharArray().Length > chars)
            {
                return name.Substring(0, chars) + "...";
            }

            return name;
        }

        /// <summary>
        /// Used to round decimal number by type
        /// </summary>
        /// <param name="number"></param>
        /// <param name="roundType"></param>
        /// <returns></returns>
        public static decimal RoundNumber(decimal number, string roundType, bool isPercent)
        {
            decimal input = number;
            if (isPercent)
            {
                switch (roundType)
                {
                    case "01": // Round Down
                        number = number < 0 && number != Math.Floor(number) ? (Math.Floor(number * 10000) + 1) / 100 : Math.Floor(number * 10000) / 100;
                        break;
                    case "02": // Round Up
                        number = number > 0 && number != Math.Floor(number) ? (Math.Floor(number * 10000) + 1) / 100 : Math.Floor(number * 10000) / 100;
                        break;
                    case "03": // Round auto
                        number = Math.Round(number * 10000) / 100;
                        break;
                }
            }
            else
            {
                switch (roundType)
                {
                    case "01":
                        // 01:小数点以下切り捨て: Round Down
                        number = number < 0 && number != Math.Floor(number) ? Math.Floor(number) + 1 : Math.Floor(number);
                        break;
                    case "02":
                        // 02：小数点第一位切り上げ: Round Up
                        number = number > 0 && number != Math.Floor(number) ? Math.Floor(number) + 1 : Math.Floor(number);
                        break;
                    case "03":
                        // 03：小数点第一位四捨五入: Round C#
                        number = Math.Round(number, MidpointRounding.AwayFromZero);
                        break;
                }
            }
            return number;
        }

        /// <summary>
        /// Format Hour
        /// </summary>
        /// <param name="x"></param>
        /// <returns>Hour</returns>
        private static string formatHour(decimal x)
        {
            if (x == 0)
            {
                return "00";
            }
            else if (x >= 10 || x <= -10)
            {
                return x.ToString();
            }
            else if (x < 10 && x > 0)
            {
                return string.Format("{0:00}", x);
            }
            else //if (x < 0 && x > -10)
            {
                return string.Format("-{0:00}", x * -1);
            }
        }

        /// <summary>
        /// Format minute
        /// </summary>
        /// <param name="x"></param>
        /// <returns>Minute</returns>
        private static string formatMinute(decimal x)
        {
            if (x == 0)
                return "00";
            else if (x < 10)
                return string.Format("{0:00}", x);
            else
                return x.ToString();
        }

        /// <summary>
        /// Get hour of decimal number
        /// </summary>
        /// <param name="inputNumber"></param>
        /// <returns></returns>
        public static string getHourOfDecimal(decimal? inputNumber)
        {
            if (inputNumber != null)
            {
                var value = (decimal)inputNumber;
                inputNumber = Math.Round(value * 100) / 100;
                var min_actual = Math.Round(Math.Floor(value) == 0 && value < 0
                                    ? ((Math.Floor(value) + 1) - value) * 60
                                    : (value - Math.Floor(value)) * 60);
                min_actual = min_actual != 0 && value < 0 ? 60 - min_actual : min_actual;
                decimal hour_actual = 0;
                if (min_actual != 0 && value < 0)
                {
                    hour_actual = Math.Floor(value) + 1;
                }
                else
                {
                    hour_actual = Math.Floor(value);
                }

                var resultHour = (hour_actual == 0 && value < 0 ? '-' + formatHour(hour_actual) : formatHour(hour_actual));
                return resultHour;
            }
            else
                return "00";
        }

        /// <summary>
        /// Get minute of decimal number
        /// </summary>
        /// <param name="inputNumber"></param>
        /// <returns></returns>
        public static string getMinuteOfDecimal(decimal? inputNumber)
        {
            if (inputNumber != null)
            {
                var value = (decimal)inputNumber;
                inputNumber = Math.Round(value * 100) / 100;
                var min_actual = Math.Round(Math.Floor(value) == 0 && value < 0
                                    ? ((Math.Floor(value) + 1) - value) * 60
                                    : (value - Math.Floor(value)) * 60);
                min_actual = min_actual != 0 && value < 0 ? 60 - min_actual : min_actual;

                var resultMin = formatMinute(min_actual);
                return resultMin;
            }
            else
                return "00";
        }
        #endregion

        #region Export file Csv
        /// <summary>
        /// Using Data form datatable to export to csv file
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="dt"></param>
        /// <param name="fileName"></param>
        public static void ExportToCsvData(Controller controller, DataTable dt, string fileName = "data.csv")
        {
            string delimiter = ",";

            controller.Response.Clear();
            controller.Response.ContentType = "text/csv;";
            controller.Response.ContentEncoding = System.Text.Encoding.UTF8;
            byte[] bom = System.Text.Encoding.UTF8.GetPreamble();
            controller.Response.BinaryWrite(bom);
            controller.Response.AppendHeader("Content-type", "application/x-download");
            controller.Response.AppendHeader("Content-disposition", string.Format("attachment; filename={0}", fileName));

            string value = "";
            var builder = new StringBuilder();

            //write the csv column headers
            for (int i = 0; i < dt.Columns.Count; i++)
            {

                value = dt.Columns[i].ColumnName;
                // Implement special handling for values that contain comma or quote
                // Enclose in quotes and double up any double quotes
                if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                else
                {
                    builder.Append(value);
                }

                controller.Response.Write(value);
                controller.Response.Write((i < dt.Columns.Count - 1) ? delimiter : Environment.NewLine);
                builder.Clear();
            }

            //write the data
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    value = row[i].ToString();
                    // Implement special handling for values that contain comma or quote
                    // Enclose in quotes and double up any double quotes
                    if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                        builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                    else
                    {
                        builder.Append(value);

                    }

                    controller.Response.Write(builder.ToString());
                    controller.Response.Write((i < dt.Columns.Count - 1) ? delimiter : Environment.NewLine);
                    builder.Clear();
                }
            }

            controller.Response.End();
        }

        /// <summary>
        /// Convert from IList<IDictionary<string,object>> to DataTable
        /// </summary>
        /// <param name="items"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static DataTable ToDateTable(IList<IDictionary<string, object>> items, string[] columns)
        {
            DataTable dataTable = new DataTable(items.ToString());

            for (int i = 0; i < columns.Length; i++)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(columns[i]);
            }

            foreach (IDictionary<string, object> item in items)
            {
                var values = new object[item.Values.Count];
                int i = 0;
                foreach (var value in item.Values)
                {
                    //Setting column names as Property names
                    values[i++] = value;
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        /// <summary>
        /// Convert from IList<object[]> to DataTable
        /// </summary>
        /// <param name="items"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static DataTable ToDateTable(IList<object[]> items, string[] columns)
        {
            DataTable dataTable = new DataTable(items.ToString());

            for (int i = 0; i < columns.Length; i++)
            {
                //Setting column names as Property names
                if (dataTable.Columns.Contains(columns[i]))
                {
                    dataTable.Columns.Add(columns[i] + " ");
                }
                else
                {
                    dataTable.Columns.Add(columns[i]);
                }
            }

            foreach (var item in items)
            {
                dataTable.Rows.Add(item);
            }
            return dataTable;
        }

        /// <summary>
        /// Convert from IList<T> to DataTable
        /// </summary>
        /// <param name="items"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static DataTable ToDataTableT<T>(IList<T> items, string[] columns)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < columns.Length; i++)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(columns[i]);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        /// <summary>
        /// Get list month from time range
        /// </summary>
        /// <param name="sStartYearMonth"></param>
        /// <param name="sEndYearMonth"></param>
        /// <returns></returns>
        public static List<string> getListMonth(string sStartYearMonth, string sEndYearMonth)
        {
            int iStartY = Convert.ToInt32(sStartYearMonth.Substring(0, 4));
            int iStartM = Convert.ToInt32(sStartYearMonth.Substring(5));
            int iEndY = Convert.ToInt32(sEndYearMonth.Substring(0, 4));
            int iEndM = Convert.ToInt32(sEndYearMonth.Substring(5));

            List<string> columns = new List<string>();

            while (iStartY < iEndY || (iStartY == iEndY && iStartM <= iEndM))
            {
                string col = string.Format("{0}/{1:00}", iStartY, iStartM);
                columns.Add(col);
                iStartM++;
                if (iStartM == 13)
                {
                    iStartM = 1;
                    iStartY++;
                }
            }
            return columns;
        }
        #endregion

        #region Download file Xlsx
        /// <summary>
        /// For Example only
        /// </summary>
        /// <param name="xlsxCreator"></param>
        /// <param name="data"></param>
        private static void CreateDataDebug(ref XlsxCreator xlsxCreator, object data)
        {
            xlsxCreator.Cell("**DATE").Value = Utility.GetCurrentDateTime();
            xlsxCreator.Cell("**NO").Value = "ADV001";
            xlsxCreator.Cell("**POST").Value = 11;
            xlsxCreator.Cell("**ADDRESS").Value = 22;
            xlsxCreator.Cell("**USERNAME").Value = 11;
            xlsxCreator.Cell("**NYUKIN").Value = 11;
            xlsxCreator.Cell("**ZENKAI").Value = 11;
            xlsxCreator.Cell("**URIAGE").Value = 11;
            xlsxCreator.Cell("**KURIKOSI").Func("=C16-B16", 11 - 22);
            xlsxCreator.Cell("**ZEI").Func("=E16*0.05", 11 * 0.05);
            xlsxCreator.Cell("**TOTAL").Func("=(C16-B16)+(E16*1.05)", (11 - 11) + (22 * 1.05));
            xlsxCreator.Cell("**BIKO").Value = "お振込は翌月の 30 日までにお願いします。";
        }

        /// <summary>
        /// Fill project plan data for downloading
        /// </summary>
        /// <param name="xlsxCreator"></param>
        /// <param name="data"></param>
        private static void CreateProjectPlan(ref XlsxCreator xlsxCreator, object data)
        {
            var projectPlan = (ProjectPlanInfoPlus)data;
            xlsxCreator.InitFormulaAnswer = true;

            // 作成者
            xlsxCreator.Cell("Z6").Value = projectPlan.user_regist;
            // 作成日
            if (projectPlan.ins_id == 0)
            {
                xlsxCreator.Cell("AF6").Value = "";
            }
            else
            {
                xlsxCreator.Cell("AF6").Value = projectPlan.ins_date;
            }

            // 依頼元
            xlsxCreator.Cell("B9").Value = projectPlan.customer_name;
            // プロジェクト名
            xlsxCreator.Cell("K9").Value = projectPlan.project_name;
            // ＰＭ
            xlsxCreator.Cell("AD9").Value = projectPlan.person_in_charge;
            // 営業窓口
            xlsxCreator.Cell("AH9").Value = projectPlan.sales_person_in_charge;

            // 現状の課題
            xlsxCreator.Cell("B13").Value = projectPlan.issues;
            // 目的
            xlsxCreator.Cell("T13").Value = projectPlan.purpose;

            // 目標（目標を達成するためのアウトプット）
            xlsxCreator.Cell("D15").Value = projectPlan.target_01;
            xlsxCreator.Cell("D16").Value = projectPlan.target_02;
            xlsxCreator.Cell("D17").Value = projectPlan.target_03;
            xlsxCreator.Cell("D18").Value = projectPlan.target_04;
            xlsxCreator.Cell("D19").Value = projectPlan.target_05;

            xlsxCreator.Cell("V15").Value = projectPlan.target_06;
            xlsxCreator.Cell("V16").Value = projectPlan.target_07;
            xlsxCreator.Cell("V17").Value = projectPlan.target_08;
            xlsxCreator.Cell("V18").Value = projectPlan.target_09;
            xlsxCreator.Cell("V19").Value = projectPlan.target_10;

            // 内容
            xlsxCreator.Cell("J23").Value = projectPlan.restriction_01;
            xlsxCreator.Cell("J24").Value = projectPlan.restriction_02;
            xlsxCreator.Cell("J25").Value = projectPlan.restriction_03;
            xlsxCreator.Cell("J26").Value = projectPlan.restriction_04;
            xlsxCreator.Cell("J27").Value = projectPlan.restriction_05;
            xlsxCreator.Cell("J28").Value = projectPlan.restriction_06;

            // 懸念事項
            xlsxCreator.Cell("B37").Value = projectPlan.concerns_01;
            xlsxCreator.Cell("B38").Value = projectPlan.concerns_02;
            xlsxCreator.Cell("B39").Value = projectPlan.concerns_03;
            xlsxCreator.Cell("B40").Value = projectPlan.concerns_04;
            xlsxCreator.Cell("B41").Value = projectPlan.concerns_05;

            // 対策
            xlsxCreator.Cell("T37").Value = projectPlan.measures_01;
            xlsxCreator.Cell("T38").Value = projectPlan.measures_02;
            xlsxCreator.Cell("T39").Value = projectPlan.measures_03;
            xlsxCreator.Cell("T40").Value = projectPlan.measures_04;
            xlsxCreator.Cell("T41").Value = projectPlan.measures_05;

            // 品質管理の支援有無
            if (projectPlan.support_test_plan_flg == Constant.SupportTest.SUPPORT)
            {
                xlsxCreator.Cell("R44").Value = "✔";
            }
            if (projectPlan.support_user_test_flg == Constant.SupportTest.SUPPORT)
            {
                xlsxCreator.Cell("R45").Value = "✔";
            }
            if (projectPlan.support_stress_test_flg == Constant.SupportTest.SUPPORT)
            {
                xlsxCreator.Cell("AJ44").Value = "✔";
            }
            if (projectPlan.support_security_test_flg == Constant.SupportTest.SUPPORT)
            {
                xlsxCreator.Cell("AJ45").Value = "✔";
            }

            // その他（除外事項など）
            xlsxCreator.Cell("B48").Value = projectPlan.remarks;
        }

        /// <summary>
        /// Fill Attendance data for downloading
        /// </summary>
        /// <param name="xlsxCreator"></param>
        /// <param name="data"></param>
        private static void CreateDataAttendance(ref XlsxCreator xlsxCreator, object data)
        {
            var AttdInfo = (AttendanceInfo)data;
            xlsxCreator.InitFormulaAnswer = true;

            // Sheet Name
            xlsxCreator.SheetNo = 0;
            xlsxCreator.SheetName = string.Format("{0}年{1:00}月度", AttdInfo.SelectedYear, AttdInfo.SelectedMonth);

            // 社員番号
            xlsxCreator.Cell(ExcelCellName.EMPLOYEE_NO).Value = AttdInfo.UserInfo.employee_no;

            // 所属名
            xlsxCreator.Cell(ExcelCellName.GROUP_NAME).Value = AttdInfo.UserInfo.group_name;

            // 名前
            xlsxCreator.Cell(ExcelCellName.EMPLOYEE_NAME).Value = AttdInfo.UserInfo.display_name;

            // 拠点
            xlsxCreator.Cell(ExcelCellName.BRANCH).Value = AttdInfo.UserInfo.location_name;

            // 入社年月日
            xlsxCreator.Cell(ExcelCellName.HIRE_DATE).Value = AttdInfo.UserInfo.entry_date.HasValue ? AttdInfo.UserInfo.entry_date.Value.ToString("yyyy/MM/dd") : string.Empty;

            // 対象年月
            xlsxCreator.Cell(ExcelCellName.TARGET_YEAR).Value = AttdInfo.SelectedYear;
            xlsxCreator.Cell(ExcelCellName.TAREGET_MONTH).Value = AttdInfo.SelectedMonth;

            // 対象範囲開始日付
            DateTime targetStartDate = Convert.ToDateTime(string.Format("{0}/{1}/{2}", AttdInfo.SelectedYear, AttdInfo.SelectedMonth, AttdInfo.WorkClosingDay)).AddDays(1);
            if (AttdInfo.WorkClosingDay < 30)
            {
                // 月末締め以外の場合、対象年月を一ヶ月前に変更
                targetStartDate = targetStartDate.AddMonths(-1);
            }
            // 対象範囲終了日付
            DateTime targetEndDate = targetStartDate.AddMonths(1);

            int y = 0, i = 0;
            for (DateTime workingDate = targetStartDate; workingDate < targetEndDate; workingDate = workingDate.AddDays(1))
            {
                // 勤務日
                xlsxCreator.Cell(ExcelCellName.WORKING_DATE, 0, y).Value = workingDate;

                if (i < AttdInfo.AttDetail.Count && workingDate == AttdInfo.AttDetail[i].working_date)
                {
                    // 祝日設定
                    xlsxCreator.Cell(ExcelCellName.IS_PUBLIC_HOLIDAY, 0, y).Value =
                        string.IsNullOrEmpty(AttdInfo.AttDetail[i].holiday_name) ?
                        string.Empty : Constant.WorkingDateType.Items[Constant.WorkingDateType.PUBLIC_HOLIDAY];
                    // 勤務区分
                    xlsxCreator.Cell(ExcelCellName.ATTENDANCE_TYPE, 0, y).Value = AttdInfo.AttDetail[i].attendance_type_name;
                    // 有休時間
                    xlsxCreator.Cell(ExcelCellName.ALLOWED_COST_HOUR, 0, y).Value = AttdInfo.AttDetail[i].allowed_cost_time_hour;
                    xlsxCreator.Cell(ExcelCellName.ALLOWED_COST_MINUTE, 0, y).Value = AttdInfo.AttDetail[i].allowed_cost_time_minute;
                    // 備考
                    xlsxCreator.Cell(ExcelCellName.REMARKS, 0, y).Value = AttdInfo.AttDetail[i].remarks;
                    // i-reco
                    xlsxCreator.Cell(ExcelCellName.CLOCK_IN_START_HOUR, 0, y).Value = AttdInfo.AttDetail[i].clock_in_start_hour;
                    xlsxCreator.Cell(ExcelCellName.CLOCK_IN_START_MINUTE, 0, y).Value = AttdInfo.AttDetail[i].clock_in_start_minute;
                    xlsxCreator.Cell(ExcelCellName.CLOCK_IN_END_HOUR, 0, y).Value = AttdInfo.AttDetail[i].clock_in_end_hour;
                    xlsxCreator.Cell(ExcelCellName.CLOCK_IN_END_MINUTE, 0, y).Value = AttdInfo.AttDetail[i].clock_in_end_minute;
                    // i-Pro
                    xlsxCreator.Cell(ExcelCellName.START_HOUR, 0, y).Value = AttdInfo.AttDetail[i].work_start_hour;
                    xlsxCreator.Cell(ExcelCellName.START_MINUTE, 0, y).Value = AttdInfo.AttDetail[i].work_start_minute;
                    xlsxCreator.Cell(ExcelCellName.END_HOUR, 0, y).Value = AttdInfo.AttDetail[i].work_end_hour;
                    xlsxCreator.Cell(ExcelCellName.END_MINUTE, 0, y).Value = AttdInfo.AttDetail[i].work_end_minute;
                    // 休憩時間
                    xlsxCreator.Cell(ExcelCellName.REST_HOUR, 0, y).Value = AttdInfo.AttDetail[i].rest_hour;
                    xlsxCreator.Cell(ExcelCellName.REST_MINUTE, 0, y).Value = AttdInfo.AttDetail[i].rest_minute;

                    i++;
                }

                y++;

            }
        }

        /// <summary>
        /// Fill Sale and profit data for downloading
        /// </summary>
        private static void CreateDataSaleProfit(ref XlsxCreator xlsxCreator, object data)
        {
            Dictionary<string, object> dummyData = new Dictionary<string, object>();
            var dataBudget = (DataSalesProfitExport)data;
            var dataSalesBudget = dataBudget.data_sale;
            var dataProfitBudget = dataBudget.data_profit;
            List<Ordinate> ordinatesContentList = new List<Ordinate>();
            List<Ordinate> ordinatesTotalMonthList = new List<Ordinate>();
            List<Ordinate> ordinatesTotalCompanyList = new List<Ordinate>();
            List<Ordinate> ordinatesTotalGroupList = new List<Ordinate>();
            List<Ordinate> ordinateYearMonthList = new List<Ordinate>();

            #region get period and CheckSalesType
            string timeStart = dataSalesBudget.TimeStart.Replace("/", "年") + "月";
            string timeEnd = dataSalesBudget.TimeEnd.Replace("/", "年") + "月";
            dummyData.Add("Period", timeStart + " ～ " + timeEnd);

            if (dataSalesBudget.CheckSalesType == "1")
            {
                dummyData.Add("CheckSalesType", "見込み含む");
            }
            else
            {
                dummyData.Add("CheckSalesType", "");
            }
            #endregion

            #region get and process monthList
            List<TimeList> timeList = new List<TimeList>();
            foreach (TimeList objTimeSales in dataSalesBudget.TimeList)
            {
                timeList.Add(objTimeSales);
            }
            foreach (TimeList objTimeProfit in dataProfitBudget.TimeListProfit)
            {
                timeList.Add(objTimeProfit);
            }

            if (timeList != null)
            {
                for (int i = 0; i < timeList.Count - 1; i++)
                {
                    for (int j = i + 1; j < timeList.Count; j++)
                    {
                        if (timeList[i].month == timeList[j].month && timeList[i].year == timeList[j].year)
                        {
                            timeList.Remove(timeList[j]);
                        }
                    }
                }
            }

            TimeList tmp;
            for (int i = 0; i < timeList.Count; i++)
            {
                for (int j = i + 1; j < timeList.Count; j++)
                {
                    if (Convert.ToInt32(timeList[j].month) < Convert.ToInt32(timeList[i].month) && Convert.ToInt32(timeList[j].year) <= Convert.ToInt32(timeList[i].year))
                    {
                        tmp = timeList[i];
                        timeList[i] = timeList[j];
                        timeList[j] = tmp;
                    }
                }
            }
            List<string> monthList = new List<string>();
            foreach (TimeList obj in timeList)
            {
                monthList.Add(obj.year + "年 " + obj.month + "月");
            }
            #endregion

            #region get group,contract when search
            List<SelectListItem> groupSearch = new List<SelectListItem>();
            foreach (var gr in dataSalesBudget.Condition.List_Group)
            {
                groupSearch.Add(new SelectListItem() { Text = gr.display_name, Value = gr.group_id.ToString() });
            }
            List<SelectListItem> contractTypeSearch = new List<SelectListItem>();
            List<SelectListItem> sortCTList = new List<SelectListItem>();
            int indexCT = 0;
            foreach (var ct in dataSalesBudget.Condition.List_Contract)
            {
                indexCT++;
                contractTypeSearch.Add(new SelectListItem() { Text = ct.contract_type, Value = ct.contract_type_id.ToString() });
                sortCTList.Add(new SelectListItem() { Text = indexCT.ToString(), Value = ct.contract_type_id.ToString() });
            }

            //所属一覧
            string groupList = "";
            for (int i = 0; i < groupSearch.Count; i++)
            {
                if (i != groupSearch.Count - 1)
                {
                    groupList += groupSearch[i].Text + ",";
                }
                else
                {
                    groupList += groupSearch[i].Text;
                }
            }

            //契約種別一覧
            string contractTypeList = "";
            for (int i = 0; i < contractTypeSearch.Count; i++)
            {
                if (i != contractTypeSearch.Count - 1)
                {
                    contractTypeList += contractTypeSearch[i].Text + ",";
                }
                else
                {
                    contractTypeList += contractTypeSearch[i].Text;
                }
            }

            #endregion

            #region get list data group name, contract name
            List<SelectListItem> group = new List<SelectListItem>();
            foreach (var gr in dataSalesBudget.GroupList)
            {
                group.Add(new SelectListItem() { Text = gr.display_name, Value = gr.group_id.ToString() });
            }
            foreach (var gr in dataProfitBudget.GroupListProfit)
            {
                group.Add(new SelectListItem() { Text = gr.display_name, Value = gr.group_id.ToString() });
            }
            if (group != null)
            {
                for (int i = 0; i < group.Count - 1; i++)
                {
                    for (int j = i + 1; j < group.Count; j++)
                    {
                        if (group[i].Value == group[j].Value)
                        {
                            group.Remove(group[j]);
                        }
                    }
                }
            }
            List<SelectListItem> contractType = new List<SelectListItem>();
            List<SelectListItem> contractType_Sort = new List<SelectListItem>();
            foreach (var ct in dataSalesBudget.ContractTypeList)
            {
                contractType_Sort.Add(new SelectListItem() { Text = ct.contract_type, Value = ct.contract_type_id.ToString() });
            }
            foreach (var ct in dataProfitBudget.ContractTypeListProfit)
            {
                contractType_Sort.Add(new SelectListItem() { Text = ct.contract_type, Value = ct.contract_type_id.ToString() });
            }
            if (contractType_Sort != null)
            {
                for (int i = 0; i < contractType_Sort.Count - 1; i++)
                {
                    for (int j = i + 1; j < contractType_Sort.Count; j++)
                    {
                        if (contractType_Sort[i].Value == contractType_Sort[j].Value)
                        {
                            contractType_Sort.Remove(contractType_Sort[j]);
                        }
                    }
                }
                foreach (var itemsSort in sortCTList)
                {
                    foreach (var itemsCT in contractType_Sort)
                    {
                        if (itemsSort.Value == itemsCT.Value)
                        {
                            contractType.Add(new SelectListItem() { Text = itemsCT.Text, Value = itemsCT.Value });
                        }
                    }
                }
            }
            #endregion

            #region generate label condition
            dummyData.Add("GroupList", groupSearch);
            dummyData.Add("ContractTypeList", contractTypeSearch);
            xlsxCreator.InitFormulaAnswer = true;
            // Sheet Name
            xlsxCreator.SheetNo = 0;
            xlsxCreator.SheetName = "所属別予実一覧";
            // period
            xlsxCreator.Cell(ExcelCellName.PERIOD).Value = dummyData["Period"].ToString();
            // 見込み含む
            xlsxCreator.Cell(ExcelCellName.INCLUDING_EXPECTED).Value = dummyData["CheckSalesType"].ToString();
            // group list
            xlsxCreator.Cell(ExcelCellName.GROUP_LIST).Value = groupList;
            // contract type list
            xlsxCreator.Cell(ExcelCellName.CONTRACT_TYPE_LIST).Value = contractTypeList;
            #endregion

            #region accounting total company all budget
            #region accounting  sales budget
            string totalCompanyAllSaleBudget = "-";
            string totalCompanyAllSaleActual = "-";
            string totalCompanyAllSaleRate = "-";

            long totalAllCpSales = 0;
            long totalAllCpSalesActual = 0;

            foreach (var totalGrYearList in dataSalesBudget.TotalGrYearList)
            {
                totalAllCpSales += Convert.ToInt64(decimal.Parse(totalGrYearList.tgrBudget.ToString(), NumberStyles.Float));
                totalAllCpSalesActual += Convert.ToInt64(decimal.Parse(totalGrYearList.tgrSales.ToString(), NumberStyles.Float));
                totalCompanyAllSaleBudget = totalAllCpSales.ToString();
                totalCompanyAllSaleActual = totalAllCpSalesActual.ToString();
            }
            if (totalCompanyAllSaleBudget == "-" || totalCompanyAllSaleBudget == "0")
            {
                totalCompanyAllSaleRate = "-";
            }
            else
            {
                decimal result1 = (Convert.ToDecimal(totalCompanyAllSaleActual) / Convert.ToDecimal(totalCompanyAllSaleBudget)) * 100;
                totalCompanyAllSaleRate = (Math.Round(result1 * 100) / 100).ToString() == "0" ? "0.00" : (Math.Round(result1 * 100) / 100).ToString();
            }
            #endregion
            #region accounting profit budget
            long totalCpAllProfitBudget = 0;
            long totalCpAllProfitActual = 0;

            string totalCompanyAllProfitBudget = "-";
            string totalCompanyAllProfitActual = "-";
            string totalCompanyAllProfitRate = "-";
            string totalCompanyAllRateSuccess = "-";
            long totalCompanyAllSale = 0;
            foreach (var totalGrYearList in dataProfitBudget.TotalGrYearListProfit)
            {
                totalCpAllProfitBudget += Convert.ToInt64(decimal.Parse(totalGrYearList.tgrProfitBudget.ToString(), NumberStyles.Float));
                totalCpAllProfitActual += Convert.ToInt64(decimal.Parse(totalGrYearList.tgrProfitActual.ToString(), NumberStyles.Float));
                totalCompanyAllProfitBudget = totalCpAllProfitBudget.ToString();
                totalCompanyAllProfitActual = totalCpAllProfitActual.ToString();
                totalCompanyAllSale += Convert.ToInt64(decimal.Parse(totalGrYearList.tgrSaleActual.ToString(), NumberStyles.Float));
            }
            if (totalCompanyAllProfitBudget == "-" || totalCompanyAllProfitBudget == "0")
            {
                totalCompanyAllProfitRate = "-";
            }
            else
            {
                decimal resultAllRate = (Convert.ToDecimal(totalCompanyAllProfitActual) / Convert.ToDecimal(totalCompanyAllProfitBudget)) * 100;
                totalCompanyAllProfitRate = Math.Round(resultAllRate, 2, MidpointRounding.AwayFromZero).ToString() == "0" ? "0.00" : Math.Round(resultAllRate, 2, MidpointRounding.AwayFromZero).ToString();
            }
            if (totalCompanyAllSale == 0)
            {
                totalCompanyAllRateSuccess = "-";
            }
            else
            {
                decimal resultTotalCompanyAllSale = (Convert.ToDecimal(totalCompanyAllProfitActual) / Convert.ToDecimal(totalCompanyAllSale)) * 100;
                totalCompanyAllRateSuccess = Math.Round(resultTotalCompanyAllSale, 2, MidpointRounding.AwayFromZero).ToString() == "0" ? "0.00" : Math.Round(resultTotalCompanyAllSale, 2, MidpointRounding.AwayFromZero).ToString();
            }
            #endregion
            #endregion
            int posX = 0, posY = 0;
            #region generate header all company
            // Add column name for each group
            xlsxCreator.Cell(ExcelCellName.GROUP, posX, 0).Value = "全社";
            // Merge cell
            xlsxCreator.Pos(posX + 1, 8, posX + 7, 8).Attr.MergeCells = true; // old posX + 4
            xlsxCreator.Pos(posX + 1, 8, posX + 7, 8).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;
            // Add column Budget, actual and rate for each group and allign center
            xlsxCreator.Cell(ExcelCellName.BUDGET_SALES, posX, 0).Value = "売上予算";
            xlsxCreator.Cell(ExcelCellName.ACTUAL_SALES, posX, 0).Value = "売上実績";
            xlsxCreator.Cell(ExcelCellName.RATE_SALES, posX, 0).Value = "達成率";

            xlsxCreator.Cell(ExcelCellName.BUDGET_PROFIT, posX, 0).Value = "利益予算";
            xlsxCreator.Cell(ExcelCellName.ACTUAL_PROFIT, posX, 0).Value = "利益実績";
            xlsxCreator.Cell(ExcelCellName.RATE_PROFIT, posX, 0).Value = "達成率";
            xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_PROFIT, posX, 0).Value = "利益率";


            xlsxCreator.Cell(ExcelCellName.BUDGET_SALES, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;
            xlsxCreator.Cell(ExcelCellName.ACTUAL_SALES, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;
            xlsxCreator.Cell(ExcelCellName.RATE_SALES, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;

            xlsxCreator.Cell(ExcelCellName.BUDGET_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;
            xlsxCreator.Cell(ExcelCellName.ACTUAL_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;
            xlsxCreator.Cell(ExcelCellName.RATE_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;
            xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;

            //Add total data of all company and group and allign right
            #region check null display value all company sales actual
            xlsxCreator.Cell(ExcelCellName.BUDGET_ALL_SALES, posX, 0).Value = totalCompanyAllSaleBudget == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalCompanyAllSaleBudget)) + "円";
            xlsxCreator.Cell(ExcelCellName.ACTUAL_ALL_SALES, posX, 0).Value = totalCompanyAllSaleActual == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalCompanyAllSaleActual)) + "円";

            if (totalCompanyAllSaleRate == "-")
            {
                xlsxCreator.Cell(ExcelCellName.RATE_ALL_SALES, posX, 0).Value = totalCompanyAllSaleRate;
            }
            else
            {
                xlsxCreator.Cell(ExcelCellName.RATE_ALL_SALES, posX, 0).Attr.Format = "0.00\"%\"";
                xlsxCreator.Cell(ExcelCellName.RATE_ALL_SALES, posX, 0).Value = Convert.ToDouble(totalCompanyAllSaleRate);
            }

            #endregion
            #region check null display value all company profit actual

            xlsxCreator.Cell(ExcelCellName.BUDGET_ALL_PROFIT, posX, 0).Value = totalCompanyAllProfitBudget == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalCompanyAllProfitBudget)) + "円";
            xlsxCreator.Cell(ExcelCellName.ACTUAL_ALL_PROFIT, posX, 0).Value = totalCompanyAllProfitActual == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalCompanyAllProfitActual)) + "円";

            if (totalCompanyAllRateSuccess == "-")
            {
                xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_ALL_PROFIT, posX, 0).Value = totalCompanyAllRateSuccess;
            }
            else
            {
                xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_ALL_PROFIT, posX, 0).Attr.Format = "0.00\"%\"";
                xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_ALL_PROFIT, posX, 0).Value = Convert.ToDouble(totalCompanyAllRateSuccess);
            }

            if (totalCompanyAllProfitRate == "-")
            {
                xlsxCreator.Cell(ExcelCellName.RATE_ALL_PROFIT, posX, 0).Value = totalCompanyAllProfitRate;
            }
            else
            {
                xlsxCreator.Cell(ExcelCellName.RATE_ALL_PROFIT, posX, 0).Attr.Format = "0.00\"%\"";
                xlsxCreator.Cell(ExcelCellName.RATE_ALL_PROFIT, posX, 0).Value = Convert.ToDouble(totalCompanyAllProfitRate);
            }

            #endregion

            xlsxCreator.Cell(ExcelCellName.BUDGET_ALL_SALES, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
            xlsxCreator.Cell(ExcelCellName.ACTUAL_ALL_SALES, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
            xlsxCreator.Cell(ExcelCellName.RATE_ALL_SALES, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;

            xlsxCreator.Cell(ExcelCellName.BUDGET_ALL_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
            xlsxCreator.Cell(ExcelCellName.ACTUAL_ALL_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
            xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_ALL_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
            xlsxCreator.Cell(ExcelCellName.RATE_ALL_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
            Ordinate ordinatesTotalCompanyItem = new Ordinate() { posX = posX, posY = 0 };
            ordinatesTotalCompanyList.Add(ordinatesTotalCompanyItem);
            posX += 7;

            #endregion
            //Generate all header
            foreach (var groupName in group)
            {
                #region get total sale budget by group
                #region get by mode sales
                string totalSaleBudgetBySales = "-";
                string totalaActualBySales = "-";
                string totalRateBySales = "-";
                foreach (var totalGrYearList in dataSalesBudget.TotalGrYearList)
                {
                    if (totalGrYearList.group_id == groupName.Value)
                    {
                        totalSaleBudgetBySales = totalGrYearList.tgrBudget.ToString();
                        totalaActualBySales = totalGrYearList.tgrSales.ToString();
                        totalRateBySales = totalGrYearList.tgrBudget == "0" ? "-" : Math.Round(Convert.ToDecimal(totalGrYearList.tgrProfit == "0" ? "0.00" : totalGrYearList.tgrProfit), 2, MidpointRounding.ToEven).ToString();
                        break;
                    }
                }
                #endregion
                #region get by mode profit
                string totalSaleBudgetByProfit = "-";
                string totalaActualByProfit = "-";
                string totalRateByProfit = "-";
                string totalSuccessRate = "-";
                foreach (var totalGrYearList in dataProfitBudget.TotalGrYearListProfit)
                {
                    if (totalGrYearList.group_id == groupName.Value)
                    {
                        totalSaleBudgetByProfit = totalGrYearList.tgrProfitBudget.ToString();
                        totalaActualByProfit = totalGrYearList.tgrProfitActual.ToString();
                        totalRateByProfit = totalGrYearList.tgrProfitRate == "-" ? "-" : Math.Round(Convert.ToDecimal(totalGrYearList.tgrProfitRate == "0" ? "0.00" : totalGrYearList.tgrProfitRate), 2, MidpointRounding.AwayFromZero).ToString();
                        totalSuccessRate = totalGrYearList.tgrSuccessRate == "-" ? "-" : Math.Round(Convert.ToDecimal(totalGrYearList.tgrSuccessRate == "0" ? "0.00" : totalGrYearList.tgrSuccessRate), 2, MidpointRounding.AwayFromZero).ToString();
                        break;
                    }
                }
                #endregion
                #endregion

                #region generate data total group by group name

                // Add column name for each group
                xlsxCreator.Cell(ExcelCellName.GROUP, posX, 0).Value = groupName.Text;
                //Merge cell
                xlsxCreator.Pos(posX + 1, 8, posX + 7, 8).Attr.MergeCells = true; // old posX + 4
                xlsxCreator.Pos(posX + 1, 8, posX + 7, 8).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;
                // Add column Budget, actual and rate for each group and allign center
                xlsxCreator.Cell(ExcelCellName.BUDGET_SALES, posX, 0).Value = "売上予算";
                xlsxCreator.Cell(ExcelCellName.ACTUAL_SALES, posX, 0).Value = "売上実績";
                xlsxCreator.Cell(ExcelCellName.RATE_SALES, posX, 0).Value = "達成率";


                xlsxCreator.Cell(ExcelCellName.BUDGET_PROFIT, posX, 0).Value = "利益予算";
                xlsxCreator.Cell(ExcelCellName.ACTUAL_PROFIT, posX, 0).Value = "利益実績";
                xlsxCreator.Cell(ExcelCellName.RATE_PROFIT, posX, 0).Value = "達成率";
                xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_PROFIT, posX, 0).Value = "利益率";


                xlsxCreator.Cell(ExcelCellName.BUDGET_SALES, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;
                xlsxCreator.Cell(ExcelCellName.ACTUAL_SALES, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;
                xlsxCreator.Cell(ExcelCellName.RATE_SALES, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;

                xlsxCreator.Cell(ExcelCellName.BUDGET_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;
                xlsxCreator.Cell(ExcelCellName.ACTUAL_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;
                xlsxCreator.Cell(ExcelCellName.RATE_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;
                xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Center;

                //Add total data of all company and group and allign right
                #region check null display value all group sales actual

                xlsxCreator.Cell(ExcelCellName.BUDGET_ALL_SALES, posX, 0).Value = totalSaleBudgetBySales == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalSaleBudgetBySales)) + "円";
                xlsxCreator.Cell(ExcelCellName.ACTUAL_ALL_SALES, posX, 0).Value = totalaActualBySales == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalaActualBySales)) + "円";
                if (totalRateBySales == "-")
                {
                    xlsxCreator.Cell(ExcelCellName.RATE_ALL_SALES, posX, 0).Value = totalRateBySales;
                }
                else
                {
                    xlsxCreator.Cell(ExcelCellName.RATE_ALL_SALES, posX, 0).Attr.Format = "0.00\"%\"";
                    xlsxCreator.Cell(ExcelCellName.RATE_ALL_SALES, posX, 0).Value = Convert.ToDouble(totalRateBySales);
                }

                #endregion
                #region check null display value all group profit actual

                xlsxCreator.Cell(ExcelCellName.BUDGET_ALL_PROFIT, posX, 0).Value = totalSaleBudgetByProfit == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalSaleBudgetByProfit)) + "円";
                xlsxCreator.Cell(ExcelCellName.ACTUAL_ALL_PROFIT, posX, 0).Value = totalaActualByProfit == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalaActualByProfit)) + "円"; ;

                if (totalRateByProfit == "-")
                {
                    xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_ALL_PROFIT, posX, 0).Value = totalRateByProfit;
                }
                else
                {
                    xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_ALL_PROFIT, posX, 0).Attr.Format = "0.00\"%\"";
                    xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_ALL_PROFIT, posX, 0).Value = Convert.ToDouble(totalRateByProfit);
                }

                if (totalSuccessRate == "-")
                {
                    xlsxCreator.Cell(ExcelCellName.RATE_ALL_PROFIT, posX, 0).Value = totalSuccessRate;
                }
                else
                {
                    xlsxCreator.Cell(ExcelCellName.RATE_ALL_PROFIT, posX, 0).Attr.Format = "0.00\"%\"";
                    xlsxCreator.Cell(ExcelCellName.RATE_ALL_PROFIT, posX, 0).Value = Convert.ToDouble(totalSuccessRate);
                }

                #endregion

                xlsxCreator.Cell(ExcelCellName.BUDGET_ALL_SALES, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                xlsxCreator.Cell(ExcelCellName.ACTUAL_ALL_SALES, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                xlsxCreator.Cell(ExcelCellName.RATE_ALL_SALES, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;


                xlsxCreator.Cell(ExcelCellName.BUDGET_ALL_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                xlsxCreator.Cell(ExcelCellName.ACTUAL_ALL_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_ALL_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                xlsxCreator.Cell(ExcelCellName.RATE_ALL_PROFIT, posX, 0).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                Ordinate ordinatesTotalGroupItem = new Ordinate() { posX = posX, posY = posY };
                ordinatesTotalGroupList.Add(ordinatesTotalGroupItem);

                posX += 7;
                #endregion
            }
            //Set background and bold font for header in line 7
            xlsxCreator.Pos(0, 8, 1, 8).Attr.BackColor = Color.FromArgb(242, 242, 242);
            xlsxCreator.Pos(1, 8, posX, 9).Attr.BackColor = Color.FromArgb(242, 242, 242);
            xlsxCreator.Pos(0, 8, 1, 8).Attr.FontStyle = AdvanceSoftware.ExcelCreator.FontStyle.Bold;
            xlsxCreator.Pos(1, 8, posX, 9).Attr.FontStyle = AdvanceSoftware.ExcelCreator.FontStyle.Bold;
            xlsxCreator.Pos(1, 8, posX, 9).Attr.FontStyle = AdvanceSoftware.ExcelCreator.FontStyle.Bold;
            //Set background for data Total of all company and group
            xlsxCreator.Pos(0, 10, posX, 10).Attr.BackColor = Color.FromArgb(253, 176, 11);
            #region generate content         
            foreach (var time in timeList)
            {
                //Add month 
                xlsxCreator.Cell(ExcelCellName.MONTH, 0, posY).Value = time.year + "年 " + time.month + "月";
                Ordinate ordinateYearMonthItem = new Ordinate() { posX = posX, posY = posY };
                ordinateYearMonthList.Add(ordinateYearMonthItem);
                //Set merge cell and background color for month
                //xlsxCreator.Pos(0, posY + 11, posX, posY + 11).Attr.MergeCells = true;
                xlsxCreator.Pos(0, posY + 11, posX, posY + 11).Attr.BackColor = Color.FromArgb(151, 216, 213);

                foreach (var contractTypeName in contractType)
                {
                    #region total month by mode sales
                    string totalAllSaleBudgetBySales = "-";
                    string totalAllActualBySales = "-";
                    string totalAllRateBySales = "-";

                    string totalMonthAllSaleBudgetBySales = "-";
                    string totalMonthAllActualBySales = "-";
                    long gettotalMonthAllSaleBudgetBySales = 0;
                    long gettotalMonthAllActualBySales = 0;
                    string totalMonthAllRateBySales = "-";

                    foreach (var totalCt in dataSalesBudget.TotalCT)
                    {
                        if (totalCt.target_month == Int32.Parse(time.month)
                            && totalCt.target_year == Int32.Parse(time.year)
                            && totalCt.contract_type_id == contractTypeName.Value)
                        {
                            totalAllSaleBudgetBySales = totalCt.tgrBudget.ToString();
                            totalAllActualBySales = totalCt.tgrSales.ToString();
                            totalAllRateBySales = totalCt.tgrBudget == "0" ? "-" : Math.Round(Convert.ToDecimal(totalCt.tgrProfit == "0" ? "0.00" : totalCt.tgrProfit), 2, MidpointRounding.ToEven).ToString();
                            break;
                        }
                    }
                    #endregion
                    #region total month by mode profit
                    string totalAllProfitBudgetByProfit = "-";
                    string totalAllActualByProfit = "-";
                    string totalAllRateByProfit = "-";
                    string totalAllRateSuccess = "-";

                    string totalMonthAllProfitBudgetByProfit = "-";
                    string totalMonthAllActualByProfit = "-";
                    string totalMonthAllRateByProfit = "-";
                    string totalMonthAllRateSuccess = "-";
                    foreach (var totalCt in dataProfitBudget.TotalCTProfit)
                    {
                        if (totalCt.target_month == Int32.Parse(time.month)
                            && totalCt.target_year == Int32.Parse(time.year)
                            && totalCt.contract_type_id == contractTypeName.Value)
                        {
                            totalAllProfitBudgetByProfit = totalCt.tgrProfitBudget.ToString();
                            totalAllActualByProfit = totalCt.tgrProfitActual.ToString();
                            totalAllRateByProfit = totalCt.tgrProfitRate == "-" ? "-" : Math.Round(Convert.ToDecimal(totalCt.tgrProfitRate == "0" ? "0.00" : totalCt.tgrProfitRate), 2, MidpointRounding.AwayFromZero).ToString();
                            totalAllRateSuccess = totalCt.tgrSuccessRate == "-" ? "-" : Math.Round(Convert.ToDecimal(totalCt.tgrSuccessRate == "0" ? "0.00" : totalCt.tgrSuccessRate), 2, MidpointRounding.AwayFromZero).ToString();
                            break;
                        }
                    }
                    #endregion
                    // wait checking
                    #region checking
                    if ((totalAllSaleBudgetBySales != "-"
                      || (totalAllSaleBudgetBySales == "-" && totalAllRateBySales == "-")
                      || totalAllProfitBudgetByProfit != "-"
                      || (totalAllProfitBudgetByProfit == "-" && totalAllRateByProfit == "-")
                      || (totalAllProfitBudgetByProfit == "-" && totalAllRateSuccess == "-")))
                    {
                        if (totalAllSaleBudgetBySales == "-" && totalAllRateBySales == "-" && totalAllActualBySales == "-"
                            && totalAllProfitBudgetByProfit == "-" && totalAllActualByProfit == "-" && totalAllRateByProfit == "-" && totalAllRateSuccess == "-")
                        {
                            //do nothing
                        }
                        else
                        {
                            #region get total all month by mode sales
                            foreach (var totalCt in dataSalesBudget.TotalCT)
                            {
                                if (totalCt.target_month == Int32.Parse(time.month)
                                    && totalCt.target_year == Int32.Parse(time.year))
                                {
                                    gettotalMonthAllSaleBudgetBySales += Convert.ToInt64(decimal.Parse(totalCt.tgrBudget.ToString(), NumberStyles.Float));// Int64.Parse(totalCt.tgrBudget);
                                    gettotalMonthAllActualBySales += Convert.ToInt64(decimal.Parse(totalCt.tgrSales.ToString(), NumberStyles.Float));// Int64.Parse(totalCt.tgrSales);

                                    totalMonthAllSaleBudgetBySales = gettotalMonthAllSaleBudgetBySales.ToString();
                                    totalMonthAllActualBySales = gettotalMonthAllActualBySales.ToString();
                                }
                            }
                            #endregion
                            #region get total all month by mode profit
                            foreach (var totalCt in dataProfitBudget.TotalMonthProfit)
                            {
                                if (totalCt.target_month == Int32.Parse(time.month)
                                    && totalCt.target_year == Int32.Parse(time.year))
                                {
                                    totalMonthAllProfitBudgetByProfit = totalCt.tgrProfitBudget.ToString();
                                    totalMonthAllActualByProfit = totalCt.tgrProfitActual.ToString();
                                    totalMonthAllRateByProfit = totalCt.tgrProfitRate == "-" ? "-" : Math.Round(Convert.ToDouble(totalCt.tgrProfitRate), 2, MidpointRounding.AwayFromZero).ToString() == "0" ? "0.00" : Math.Round(Convert.ToDouble(totalCt.tgrProfitRate), 2, MidpointRounding.AwayFromZero).ToString();
                                    totalMonthAllRateSuccess = totalCt.tgrSuccessRate == "-" ? "-" : Math.Round(Convert.ToDouble(totalCt.tgrSuccessRate), 2, MidpointRounding.AwayFromZero).ToString() == "0" ? "0.00" : Math.Round(Convert.ToDouble(totalCt.tgrSuccessRate), 2, MidpointRounding.AwayFromZero).ToString();
                                    break;
                                }
                            }
                            #endregion

                            xlsxCreator.Cell(ExcelCellName.CONTRACT_TYPE, 0, posY).Value = contractTypeName.Text;
                            posX = 0;

                            #region generate data total month by mode sales

                            #region check display null value sale actual

                            xlsxCreator.Cell(ExcelCellName.BUDGET_MONTH_SALES, posX, posY).Value = totalAllSaleBudgetBySales == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalAllSaleBudgetBySales)) + "円";
                            xlsxCreator.Cell(ExcelCellName.ACTUAL_MONTH_SALES, posX, posY).Value = totalAllActualBySales == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalAllActualBySales)) + "円";

                            if (totalAllRateBySales == "-")
                            {
                                xlsxCreator.Cell(ExcelCellName.RATE_MONTH_SALES, posX, posY).Value = totalAllRateBySales;
                            }
                            else
                            {
                                xlsxCreator.Cell(ExcelCellName.RATE_MONTH_SALES, posX, posY).Attr.Format = "0.00\"%\"";
                                xlsxCreator.Cell(ExcelCellName.RATE_MONTH_SALES, posX, posY).Value = Convert.ToDouble(totalAllRateBySales);
                            }
                            //Add total data of month and align right and set background color
                            if (totalMonthAllSaleBudgetBySales == "0" || totalMonthAllSaleBudgetBySales == "-")
                            {
                                totalMonthAllRateBySales = "-";
                            }
                            else
                            {
                                decimal result2 = (Convert.ToDecimal(totalMonthAllActualBySales) / Convert.ToDecimal(totalMonthAllSaleBudgetBySales)) * 100;
                                totalMonthAllRateBySales = (Math.Round(result2 * 100) / 100).ToString() == "0" ? "0.00" : (Math.Round(result2 * 100) / 100).ToString();
                            }

                            xlsxCreator.Cell(ExcelCellName.BUDGET_TOTAL_MONTH_SALES, posX, posY).Value = totalMonthAllSaleBudgetBySales == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalMonthAllSaleBudgetBySales)) + "円";
                            xlsxCreator.Cell(ExcelCellName.ACTUAL_TOTAL_MONTH_SALES, posX, posY).Value = totalMonthAllActualBySales == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalMonthAllActualBySales)) + "円";

                            if (totalMonthAllRateBySales == "-")
                            {
                                xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_SALES, posX, posY).Value = totalMonthAllRateBySales;
                            }
                            else
                            {
                                xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_SALES, posX, posY).Attr.Format = "0.00\"%\"";
                                xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_SALES, posX, posY).Value = Convert.ToDouble(totalMonthAllRateBySales);
                            }

                            #endregion

                            xlsxCreator.Cell(ExcelCellName.BUDGET_MONTH_SALES, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                            xlsxCreator.Cell(ExcelCellName.ACTUAL_MONTH_SALES, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                            xlsxCreator.Cell(ExcelCellName.RATE_MONTH_SALES, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;

                            xlsxCreator.Cell(ExcelCellName.BUDGET_TOTAL_MONTH_SALES, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                            xlsxCreator.Cell(ExcelCellName.ACTUAL_TOTAL_MONTH_SALES, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                            xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_SALES, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                            Ordinate ordinatesTotalMonthItem = new Ordinate() { posX = posX, posY = posY };
                            ordinatesTotalMonthList.Add(ordinatesTotalMonthItem);

                            #endregion

                            #region generate data total month by mode profit
                            //Add data of all company contract
                            #region check null display value profit actual

                            xlsxCreator.Cell(ExcelCellName.BUDGET_MONTH_PROFIT, posX, posY).Value = totalAllProfitBudgetByProfit == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalAllProfitBudgetByProfit)) + "円";
                            xlsxCreator.Cell(ExcelCellName.ACTUAL_MONTH_PROFIT, posX, posY).Value = totalAllActualByProfit == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalAllActualByProfit)) + "円";

                            if (totalAllRateByProfit == "-")
                            {
                                xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_MONTH_PROFIT, posX, posY).Value = totalAllRateByProfit;
                            }
                            else
                            {
                                xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_MONTH_PROFIT, posX, posY).Attr.Format = "0.00\"%\"";
                                xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_MONTH_PROFIT, posX, posY).Value = Convert.ToDouble(totalAllRateByProfit);
                            }

                            if (totalAllRateSuccess == "-")
                            {
                                xlsxCreator.Cell(ExcelCellName.RATE_MONTH_PROFIT, posX, posY).Value = totalAllRateSuccess;
                            }
                            else
                            {
                                xlsxCreator.Cell(ExcelCellName.RATE_MONTH_PROFIT, posX, posY).Attr.Format = "0.00\"%\"";
                                xlsxCreator.Cell(ExcelCellName.RATE_MONTH_PROFIT, posX, posY).Value = Convert.ToDouble(totalAllRateSuccess);
                            }


                            //Add total data of month and align right and set background color                     
                            xlsxCreator.Cell(ExcelCellName.BUDGET_TOTAL_MONTH_PROFIT, posX, posY).Value = totalMonthAllProfitBudgetByProfit == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalMonthAllProfitBudgetByProfit)) + "円";
                            xlsxCreator.Cell(ExcelCellName.ACTUAL_TOTAL_MONTH_PROFIT, posX, posY).Value = totalMonthAllActualByProfit == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalMonthAllActualByProfit)) + "円";

                            if (totalMonthAllRateByProfit == "-")
                            {
                                xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_TOTAL_MONTH_PROFIT, posX, posY).Value = totalMonthAllRateByProfit;
                            }
                            else
                            {
                                xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_TOTAL_MONTH_PROFIT, posX, posY).Attr.Format = "0.00\"%\"";
                                xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_TOTAL_MONTH_PROFIT, posX, posY).Value = Convert.ToDouble(totalMonthAllRateByProfit);
                            }

                            if (totalMonthAllRateSuccess == "-")
                            {
                                xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_PROFIT, posX, posY).Value = totalMonthAllRateSuccess;
                            }
                            else
                            {
                                xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_PROFIT, posX, posY).Attr.Format = "0.00\"%\"";
                                xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_PROFIT, posX, posY).Value = Convert.ToDouble(totalMonthAllRateSuccess);
                            }
                            #endregion
                            xlsxCreator.Cell(ExcelCellName.BUDGET_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                            xlsxCreator.Cell(ExcelCellName.ACTUAL_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                            xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                            xlsxCreator.Cell(ExcelCellName.RATE_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;

                            xlsxCreator.Cell(ExcelCellName.BUDGET_TOTAL_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                            xlsxCreator.Cell(ExcelCellName.ACTUAL_TOTAL_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                            xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_TOTAL_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                            xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                            // end Add data of all company contract
                            #endregion
                            posX += 7;

                            foreach (var groupName in group)
                            {
                                #region get data content for mode sales budget
                                string sales_budgetBySales = "-";
                                string actualBySales = "-";
                                string rateBySales = "-";
                                string totalSaleBudgetBySales = "-";
                                string totalaActualBySales = "-";
                                string totalRateBySales = "-";

                                foreach (var obj in dataSalesBudget.DataSalesBudget)
                                {
                                    if (obj.target_month == Int32.Parse(time.month)
                                        && obj.target_year == Int32.Parse(time.year)
                                        && obj.contract_type_id == contractTypeName.Value
                                        && obj.group_id == groupName.Value)
                                    {
                                        sales_budgetBySales = obj.sales_budget.ToString();
                                        actualBySales = obj.sales_actuals.ToString();
                                        rateBySales = sales_budgetBySales == "0" ? "-" : Math.Round(Convert.ToDecimal(obj.profit == "0" ? "0.00" : obj.profit), 2, MidpointRounding.ToEven).ToString();
                                        break;
                                    }

                                }

                                foreach (var totalGroup in dataSalesBudget.TotalGroup)
                                {
                                    if (totalGroup.group_id == groupName.Value
                                        && totalGroup.target_month == Int32.Parse(time.month)
                                        && totalGroup.target_year == Int32.Parse(time.year))
                                    {
                                        totalSaleBudgetBySales = totalGroup.tgrBudget.ToString();// Int64.Parse(totalGroup.tgrBudget);
                                        totalaActualBySales = totalGroup.tgrSales.ToString();// Int64.Parse(totalGroup.tgrSales);
                                        totalRateBySales = totalGroup.tgrBudget == "0" ? "-" : Math.Round(Convert.ToDecimal(totalGroup.tgrProfit == "0" ? "0.00" : totalGroup.tgrProfit), 2, MidpointRounding.ToEven).ToString();
                                        break;
                                    }
                                }

                                #endregion
                                #region get data content for mode profit budget 
                                string profit_budgetByProfit = "-";
                                string actualByProfit = "-";
                                string rateByProfit = "-";
                                string rateOfGainByProfit = "-";

                                string totalSaleBudgetByProfit = "-";
                                string totalaActualByProfit = "-";
                                string totalRateByProfit = "-";
                                string totalRateOfGainByProfit = "-";

                                foreach (var totalGroup in dataProfitBudget.TotalGroupProfit)
                                {
                                    if (totalGroup.group_id == groupName.Value
                                        && totalGroup.target_month == Int32.Parse(time.month)
                                        && totalGroup.target_year == Int32.Parse(time.year))
                                    {
                                        totalSaleBudgetByProfit = totalGroup.tgrProfitBudget.ToString();
                                        totalaActualByProfit = totalGroup.tgrProfitActual.ToString();
                                        totalRateByProfit = totalGroup.tgrProfitRate == "-" ? "-" : Math.Round(Convert.ToDecimal(totalGroup.tgrProfitRate == "0" ? "0.00" : totalGroup.tgrProfitRate), 2, MidpointRounding.AwayFromZero).ToString();
                                        totalRateOfGainByProfit = totalGroup.tgrSuccessRate == "-" ? "-" : Math.Round(Convert.ToDecimal(totalGroup.tgrSuccessRate == "0" ? "0.00" : totalGroup.tgrSuccessRate), 2, MidpointRounding.AwayFromZero).ToString();
                                        break;
                                    }
                                }

                                foreach (var obj in dataProfitBudget.DataProfitBudget)
                                {
                                    if (obj.target_month == Int32.Parse(time.month)
                                        && obj.target_year == Int32.Parse(time.year)
                                        && obj.contract_type_id == Convert.ToInt32(contractTypeName.Value)
                                        && obj.group_id == Convert.ToInt32(groupName.Value))
                                    {
                                        profit_budgetByProfit = obj.profit_budget.ToString();
                                        actualByProfit = obj.profit_actual.ToString();
                                        rateByProfit = obj.tgrProfitRate == "-" ? "-" : Math.Round(Convert.ToDecimal(obj.tgrProfitRate == "0" ? "0.00" : obj.tgrProfitRate), 2, MidpointRounding.AwayFromZero).ToString();
                                        rateOfGainByProfit = obj.tgrSuccessRate == "-" ? "-" : Math.Round(Convert.ToDecimal(obj.tgrSuccessRate == "0" ? "0.00" : obj.tgrSuccessRate), 2, MidpointRounding.AwayFromZero).ToString();
                                        break;
                                    }
                                }
                                #endregion

                                #region generate data content for mode sales budget
                                //Add data of each group and align right
                                #region check null display value sales actual content  

                                xlsxCreator.Cell(ExcelCellName.BUDGET_MONTH_SALES, posX, posY).Value = sales_budgetBySales == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(sales_budgetBySales)) + "円";
                                xlsxCreator.Cell(ExcelCellName.ACTUAL_MONTH_SALES, posX, posY).Value = actualBySales == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(actualBySales)) + "円";

                                if (rateBySales == "-")
                                {
                                    xlsxCreator.Cell(ExcelCellName.RATE_MONTH_SALES, posX, posY).Value = "-";
                                }
                                else
                                {
                                    xlsxCreator.Cell(ExcelCellName.RATE_MONTH_SALES, posX, posY).Attr.Format = "0.00\"%\"";
                                    xlsxCreator.Cell(ExcelCellName.RATE_MONTH_SALES, posX, posY).Value = Convert.ToDouble(rateBySales);
                                }

                                //Add total data of month and align right and set background color
                                xlsxCreator.Cell(ExcelCellName.BUDGET_TOTAL_MONTH_SALES, posX, posY).Value = totalSaleBudgetBySales == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalSaleBudgetBySales)) + "円";
                                xlsxCreator.Cell(ExcelCellName.ACTUAL_TOTAL_MONTH_SALES, posX, posY).Value = totalaActualBySales == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalaActualBySales)) + "円";
                                if (totalRateBySales == "-")
                                {
                                    xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_SALES, posX, posY).Value = "-";
                                }
                                else
                                {
                                    xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_SALES, posX, posY).Attr.Format = "0.00\"%\"";
                                    xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_SALES, posX, posY).Value = Convert.ToDouble(totalRateBySales);
                                }
                                #endregion
                                xlsxCreator.Cell(ExcelCellName.BUDGET_MONTH_SALES, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                                xlsxCreator.Cell(ExcelCellName.ACTUAL_MONTH_SALES, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                                xlsxCreator.Cell(ExcelCellName.RATE_MONTH_SALES, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;

                                xlsxCreator.Cell(ExcelCellName.BUDGET_TOTAL_MONTH_SALES, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                                xlsxCreator.Cell(ExcelCellName.ACTUAL_TOTAL_MONTH_SALES, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                                xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_SALES, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;

                                #endregion
                                #region generate data content for mode profit budget
                                //Add data of each group and align right

                                #region chech null display value profit actual content
                                xlsxCreator.Cell(ExcelCellName.BUDGET_MONTH_PROFIT, posX, posY).Value = profit_budgetByProfit == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(profit_budgetByProfit)) + "円";
                                xlsxCreator.Cell(ExcelCellName.ACTUAL_MONTH_PROFIT, posX, posY).Value = actualByProfit == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(actualByProfit)) + "円";
                                if (rateByProfit == "-")
                                {
                                    xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_MONTH_PROFIT, posX, posY).Value = "-";
                                }
                                else
                                {
                                    xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_MONTH_PROFIT, posX, posY).Attr.Format = "0.00\"%\"";
                                    xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_MONTH_PROFIT, posX, posY).Value = Convert.ToDouble(rateByProfit);
                                }

                                if (rateOfGainByProfit == "-")
                                {
                                    xlsxCreator.Cell(ExcelCellName.RATE_MONTH_PROFIT, posX, posY).Value = "-";
                                }
                                else
                                {
                                    xlsxCreator.Cell(ExcelCellName.RATE_MONTH_PROFIT, posX, posY).Attr.Format = "0.00\"%\"";
                                    xlsxCreator.Cell(ExcelCellName.RATE_MONTH_PROFIT, posX, posY).Value = Convert.ToDouble(rateOfGainByProfit);
                                }

                                //Add total data of month and align right and set background color
                                xlsxCreator.Cell(ExcelCellName.BUDGET_TOTAL_MONTH_PROFIT, posX, posY).Value = totalSaleBudgetByProfit == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalSaleBudgetByProfit)) + "円";
                                xlsxCreator.Cell(ExcelCellName.ACTUAL_TOTAL_MONTH_PROFIT, posX, posY).Value = totalaActualByProfit == "-" ? "-" : string.Format("{0:n0}", Convert.ToInt64(totalaActualByProfit)) + "円";

                                if (totalRateByProfit == "-")
                                {
                                    xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_TOTAL_MONTH_PROFIT, posX, posY).Value = "-";
                                }
                                else
                                {
                                    xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_TOTAL_MONTH_PROFIT, posX, posY).Attr.Format = "0.00\"%\"";
                                    xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_TOTAL_MONTH_PROFIT, posX, posY).Value = Convert.ToDouble(totalRateByProfit);
                                }


                                if (totalRateOfGainByProfit == "-")
                                {
                                    xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_PROFIT, posX, posY).Value = "-";
                                }
                                else
                                {
                                    xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_PROFIT, posX, posY).Attr.Format = "0.00\"%\"";
                                    xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_PROFIT, posX, posY).Value = Convert.ToDouble(totalRateOfGainByProfit);
                                }
                                #endregion

                                xlsxCreator.Cell(ExcelCellName.BUDGET_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                                xlsxCreator.Cell(ExcelCellName.ACTUAL_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                                xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                                xlsxCreator.Cell(ExcelCellName.RATE_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;

                                xlsxCreator.Cell(ExcelCellName.BUDGET_TOTAL_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                                xlsxCreator.Cell(ExcelCellName.ACTUAL_TOTAL_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                                xlsxCreator.Cell(ExcelCellName.ACHIEVE_RATE_TOTAL_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                                xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_PROFIT, posX, posY).Attr.HorizontalAlignment = AdvanceSoftware.ExcelCreator.HorizontalAlignment.Right;
                                #endregion

                                Ordinate ordinateContentItem = new Ordinate() { posX = posX, posY = posY };
                                ordinatesContentList.Add(ordinateContentItem);
                                posX += 7;
                            }
                            posY++;
                        }
                    }
                    #endregion
                }
                //Add label total month and set background
                xlsxCreator.Cell(ExcelCellName.TOTAL_MONTH, 0, posY - 1).Value = "小計";
                //Add color for total data of month
                for (int i = 0; i <= (group.Count + 1) * 7; i++) // old (group.Count) * 4
                {
                    xlsxCreator.Cell(ExcelCellName.TOTAL_MONTH, i, posY - 1).Attr.BackColor = Color.FromArgb(255, 217, 136);
                }
                posY += 2;
            }
            #endregion
            //Set table style all bordered
            xlsxCreator.Pos(0, 8, posX, posY + 10).Attr.Box(AdvanceSoftware.ExcelCreator.BoxType.Ltc, AdvanceSoftware.ExcelCreator.BorderStyle.Thin, AdvanceSoftware.ExcelCreator.xlColor.Black);
            xlsxCreator.Pos(0, 8, posX, posY + 10).Attr.FontName = "ＭＳ Ｐゴシック";
            xlsxCreator.Pos(0, 8, posX, posY + 10).Attr.ShrinkToFit = true;
            #region set line right for content sales
            if (ordinatesContentList != null)
            {
                foreach (var item in ordinatesContentList)
                {
                    xlsxCreator.Cell(ExcelCellName.RATE_MONTH_SALES, item.posX, item.posY).Attr.LineRight(AdvanceSoftware.ExcelCreator.BorderStyle.Double, AdvanceSoftware.ExcelCreator.xlColor.Black);
                    xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_SALES, item.posX, item.posY).Attr.LineRight(AdvanceSoftware.ExcelCreator.BorderStyle.Double, AdvanceSoftware.ExcelCreator.xlColor.Black);
                }
            }
            #endregion

            #region set line right for total month
            if (ordinatesTotalMonthList != null)
            {
                foreach (var item in ordinatesTotalMonthList)
                {
                    xlsxCreator.Cell(ExcelCellName.RATE_MONTH_SALES, item.posX, item.posY).Attr.LineRight(AdvanceSoftware.ExcelCreator.BorderStyle.Double, AdvanceSoftware.ExcelCreator.xlColor.Black);
                    xlsxCreator.Cell(ExcelCellName.RATE_TOTAL_MONTH_SALES, item.posX, item.posY).Attr.LineRight(AdvanceSoftware.ExcelCreator.BorderStyle.Double, AdvanceSoftware.ExcelCreator.xlColor.Black);
                }
            }
            #endregion

            #region set line right for total all company
            if (ordinatesTotalCompanyList != null)
            {
                foreach (var item in ordinatesTotalCompanyList)
                {
                    xlsxCreator.Cell(ExcelCellName.RATE_SALES, item.posX, 0).Attr.LineRight(AdvanceSoftware.ExcelCreator.BorderStyle.Double, AdvanceSoftware.ExcelCreator.xlColor.Black);
                    xlsxCreator.Cell(ExcelCellName.RATE_ALL_SALES, item.posX, 0).Attr.LineRight(AdvanceSoftware.ExcelCreator.BorderStyle.Double, AdvanceSoftware.ExcelCreator.xlColor.Black);
                }
            }
            #endregion

            #region set line right for total group
            if (ordinatesTotalGroupList != null)
            {
                foreach (var item in ordinatesTotalGroupList)
                {
                    xlsxCreator.Cell(ExcelCellName.RATE_SALES, item.posX, 0).Attr.LineRight(AdvanceSoftware.ExcelCreator.BorderStyle.Double, AdvanceSoftware.ExcelCreator.xlColor.Black);
                    xlsxCreator.Cell(ExcelCellName.RATE_ALL_SALES, item.posX, 0).Attr.LineRight(AdvanceSoftware.ExcelCreator.BorderStyle.Double, AdvanceSoftware.ExcelCreator.xlColor.Black);
                }
            }
            #endregion

            #region Remove border of year month row
            if (ordinateYearMonthList != null)
            {
                foreach (var item in ordinateYearMonthList)
                {
                    xlsxCreator.Pos(0, item.posY + 11, item.posX, item.posY + 11).Attr.LineTop(AdvanceSoftware.ExcelCreator.BorderStyle.Thin, AdvanceSoftware.ExcelCreator.xlColor.Black);
                    xlsxCreator.Pos(0, item.posY + 11, item.posX, item.posY + 11).Attr.LineBottom(AdvanceSoftware.ExcelCreator.BorderStyle.Thin, AdvanceSoftware.ExcelCreator.xlColor.Black);
                    xlsxCreator.Pos(0, item.posY + 11, item.posX, item.posY + 11).Attr.LineLeft(AdvanceSoftware.ExcelCreator.BorderStyle.None, AdvanceSoftware.ExcelCreator.xlColor.Black);
                    xlsxCreator.Pos(0, item.posY + 11, item.posX, item.posY + 11).Attr.LineRight(AdvanceSoftware.ExcelCreator.BorderStyle.None, AdvanceSoftware.ExcelCreator.xlColor.Black);
                }
            }
            #endregion
            #region set line right bold for total all company and each group
            xlsxCreator.Pos(7, 8, 7, posY + 10).Attr.LineRight(AdvanceSoftware.ExcelCreator.BorderStyle.Medium, AdvanceSoftware.ExcelCreator.xlColor.Black);
            for (int i = 2; i <= group.Count; i++)
            {
                xlsxCreator.Pos(7 * i, 8, 7 * i, posY + 10).Attr.LineRight(AdvanceSoftware.ExcelCreator.BorderStyle.Medium, AdvanceSoftware.ExcelCreator.xlColor.Black);
            }
            xlsxCreator.Pos(7 * (group.Count + 1), 8, 7 * (group.Count + 1), posY + 10).Attr.LineRight(AdvanceSoftware.ExcelCreator.BorderStyle.Thin, AdvanceSoftware.ExcelCreator.xlColor.Black);
            #endregion
        }

        /// <summary>
        /// Create file and write respond
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        /// <param name="templateFileName"></param>
        /// <param name="OutputFileName"></param>
        public static void DownloadXlsxFile(Controller controller, object data, string templateFileName, string OutputFileName = "Output.xlsx")
        {
            // Excel ファイル関連
            string strInFileName;    // オーバーレイ元の Excel ファイル(パス+ファイル名)
            string strPath = HttpContext.Current.Server.MapPath(controller.Request.ApplicationPath);

            // 生成する Excel ファイル名等の設定(※実行環境の構成に従い変更して下さい)
            strInFileName = strPath + @"\App_Data\Data\" + templateFileName;        // オーバーレイ元ファイル名

            // 売上伝票(オーバーレイ)ファイルオープン
            XlsxCreator xlsxCreator = new XlsxCreator();
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            try
            {
                xlsxCreator.OpenBook(memoryStream, strInFileName);
                // Fill data to template file
                switch (templateFileName)
                {
                    case "【○△□案件】プロジェクト計画書.xlsx":
                        CreateProjectPlan(ref xlsxCreator, data);
                        break;
                    case "勤務表.xlsx":
                        CreateDataAttendance(ref xlsxCreator, data);
                        break;
                    case "所属別予実一覧.xlsx":
                        CreateDataSaleProfit(ref xlsxCreator, data);
                        break;
                    default:
                        CreateDataDebug(ref xlsxCreator, data);
                        break;
                }
            }
            catch (Exception ex)
            {
                controller.Response.Clear();
                controller.Response.Write("ファイルの作成に失敗しました。<br /><br />");
                controller.Response.Write("エラー情報: " + ex.Message.ToString() + "<br /><br />");
                controller.Response.Write("ブラウザの戻るボタンでスタートページに戻ってください。");
                controller.Response.End();
                return;
            }

            Regex rgx = new Regex("[\\/:*?\"<>|]");
            OutputFileName = rgx.Replace(OutputFileName, "-");
            if (controller.Request.Browser.Browser.Equals("InternetExplorer"))
            {
                OutputFileName = HttpUtility.UrlPathEncode(OutputFileName);
            }
            // ファイルクローズ
            xlsxCreator.CloseBook(true);
            // 出力
            controller.Response.Clear();
            controller.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            controller.Response.AddHeader("content-disposition", "attachment; filename=\"" + OutputFileName + "\"");
            controller.Response.BinaryWrite(memoryStream.ToArray());

            memoryStream.Close();
            xlsxCreator.Dispose();
            controller.Response.End();
        }
        #endregion

        #region ServerDateTime

        /// <summary>
        /// Function to convert time zone Japanese
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static DateTime ConvertDateTimeToJst(DateTime target)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(target.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));
        }

        /// <summary>
        /// Get current Date and Time
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentDateTime()
        {
            return ConvertDateTimeToJst(DateTime.Now);
        }

        /// <summary>
        /// Get last day of month
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        }

        #endregion

        #region ValidData
        public static bool CheckEmailValid(string emailAddress)
        {
            return Regex.IsMatch(emailAddress, @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", RegexOptions.IgnoreCase);
        }

        public static bool CheckURLValid(string url)
        {
            return Regex.Match(url, @"^[a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]*$").Success;
        }

        public static bool ChecFullHalfSizeValid(string value)
        {
            return Regex.Match(value, @"^[a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]*$").Success;
        }

        public static bool CheckPhoneNumberValid(string number)
        {
            return Regex.Match(number, @"^[0-9\-]+$").Success;
        }

        #endregion

        #region Other
        /// <summary>
        /// Get name list of 12 months
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetMonthList()
        {
            var monthList = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                SelectListItem month = new SelectListItem();
                month.Value = i.ToString();
                month.Text = i + "月";
                monthList.Add(month);
            }
            return monthList;
        }

        /// <summary>
        /// Get quarter list
        /// </summary>
        /// <param name="closeMonth"></param>
        /// <returns></returns>
        public static List<SelectListItem> QuarterList(int closeMonth)
        {
            var quarterList = new List<SelectListItem>();
            List<string> monthList = new List<string>();
            if (closeMonth < 12)
            {
                for (int i = closeMonth + 1; i <= 12; i++)
                {
                    monthList.Add(i.ToString());
                }
                for (int i = 1; i <= closeMonth; i++)
                {
                    monthList.Add(i.ToString());
                }
            }
            else if (closeMonth == 12)
            {
                for (int i = 1; i <= closeMonth; i++)
                {
                    monthList.Add(i.ToString());
                }
            }
            else
            {
                return null;
            }
            if (monthList != null)
            {
                for (int i = 0; i < monthList.Count; i = i + 3)
                {
                    SelectListItem quarter = new SelectListItem();
                    if (i == 0) { quarter.Value = monthList[0] + "," + monthList[1] + "," + monthList[2]; quarter.Text = Constant.Quarter.QuarterText.First_Quarter; }
                    if (i == 3) { quarter.Value = monthList[3] + "," + monthList[4] + "," + monthList[5]; quarter.Text = Constant.Quarter.QuarterText.Second_Quarter; }
                    if (i == 6) { quarter.Value = monthList[6] + "," + monthList[7] + "," + monthList[8]; quarter.Text = Constant.Quarter.QuarterText.Third_Quarter; }
                    if (i == 9) { quarter.Value = monthList[9] + "," + monthList[10] + "," + monthList[11]; quarter.Text = Constant.Quarter.QuarterText.Fourth_Quarter; }
                    quarterList.Add(quarter);
                }
                return quarterList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get current financial year
        /// </summary>
        /// <param name="closingMonth"></param>
        /// <returns></returns>
        public static int GetCurrentFinancialYear(int closingMonth)
        {
            int currentYear = DateTime.Now.Year;
            if (closingMonth != 12)
            {
                int currentMonth = DateTime.Now.Month;
                string currentYearMonth = currentYear.ToString() + currentMonth.ToString("00");
                string currentClosingMonth = currentYear.ToString() + closingMonth.ToString("00");
                if (int.Parse(currentYearMonth) <= int.Parse(currentClosingMonth))
                {
                    return (currentYear - 1);
                }
                else
                {
                    return currentYear;
                }
            }
            else
            {
                return currentYear;
            }
        }

        /// <summary>
        /// Cloning an object of reference type
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object DeepClone(object obj)
        {
            object objResult = null;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);

                ms.Position = 0;
                objResult = bf.Deserialize(ms);
            }
            return objResult;
        }

        /// <summary>
        /// Serialize List of dynamic object To Json list
        /// </summary>
        /// <param name="listDynamic"></param>
        /// <returns></returns>
        public static IList<string> SerializeDynamicToJson(IList<dynamic> listDynamic)
        {
            List<string> result = new List<string>();
            foreach (var obj in listDynamic)
            {
                var jsonStr = JsonConvert.SerializeObject(obj);
                result.Add(jsonStr);
            }
            return result as IList<string>;
        }

        /// <summary>
        /// Deserialize Json list To Dynamic object list
        /// </summary>
        /// <param name="listJsonStr"></param>
        /// <returns></returns>
        public static IList<dynamic> DeserializeJsonToDynamic(IList<string> listJsonStr)
        {
            List<dynamic> result = new List<dynamic>();
            foreach (var jsonStr in listJsonStr)
            {
                var expObj = JsonConvert.DeserializeObject<ExpandoObject>(jsonStr);
                result.Add(expObj);
            }
            return result as IList<dynamic>;
        }
        #endregion
    }
}