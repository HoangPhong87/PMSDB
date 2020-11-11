#region Licence
// //--------------------------------------------------------------------------------------------------------------------
// //<copyright file="PMS06002ActualWorkRegist.cs" company="i-Enter Asia">
// // Copyright © 2014 i-Enter Asia. All rights reserved.
// //</copyright>
// //<project>Project Management System</project>
// //<author>Nguyen Minh Hien</author>
// //<email>hiennm@live.com</email>
// //<createdDate>23-05-2014</createdDate>
// //<summary>
// // TODO: Update summary.
// //</summary>
// //--------------------------------------------------------------------------------------------------------------------
#endregion

namespace ProjectManagementSystem.ViewModels.PMS06002
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Mvc;
    using ProjectManagementSystem.Models.PMS06002;

    /// <summary>
    /// ModelView class for the ActualWorkRegist screen
    /// </summary>
    public class PMS06002ActualWorkRegistModel
    {
        public int? target_month { get; set; }

        public int? target_year { get; set; }

        public int? user_sys_id { get; set; }

        public string company_code { get; set; }

        public string regist_type { get; set; }

        public UserWorkInfoPlus UserWorkInfo { get; set; }

        public UpdateInfo UpdateInfo  {get; set;}

        public HolidayInfo HolidayInfo { get; set; }

        public IList<ActualWorkPlus> ActualWorkList { get; set; }

        /// <summary>
        /// A list of actuak work on each day of month
        /// </summary>
        public IList<WorkAttendanceDetailPlus> WorkAttendanceDetail { get; set; }

        /// <summary>
        /// List of Attendance
        /// </summary>
        public IList<SelectListItem> AttendanceTypeList { get; set; }

        /// <summary>
        /// Generate the html content to increase the rendering speed
        /// </summary>
        public string HtmlContent
        {
            get
            {
                var sb = new StringBuilder();
                var k = 0;
                foreach (var actualWork in ActualWorkList)
                {
                    int i = 1;
                    var trClass = k % 2 == 0 ? "odd" : "even";
                    sb.Append("<tr class='tr-work-time " + trClass + "' project_sys_id = '" + actualWork.project_sys_id + "' phase_id = '" + actualWork.phase_id + "'>");
                    foreach (var workDetail in actualWork.workDetails)
                    {
                        DateTime d = new DateTime((int)actualWork.target_year, (int)actualWork.target_month, i);
                        
                        if ((HolidayInfo.special_holiday.Contains(d) || HolidayInfo.weekly_holiday.Contains(d.DayOfWeek)) 
                            && d.DayOfWeek == DayOfWeek.Saturday)
                        {
                            sb.Append("<td class=\"holiday saturday\">");
                        }
                        else if ((HolidayInfo.special_holiday.Contains(d) || HolidayInfo.weekly_holiday.Contains(d.DayOfWeek)) 
                            && d.DayOfWeek == DayOfWeek.Sunday)
                        {
                            sb.Append("<td class=\"holiday sunday\">");
                        }
                        else if (HolidayInfo.special_holiday.Contains(d) || HolidayInfo.weekly_holiday.Contains(d.DayOfWeek))
                        {
                            sb.Append("<td class=\"holiday\">");
                        }
                        else if (d.DayOfWeek == DayOfWeek.Saturday)
                        {
                            sb.Append("<td class=\"saturday\">");
                        }
                        else if (d.DayOfWeek == DayOfWeek.Sunday)
                        {
                            sb.Append("<td class=\"sunday\">");
                        }
                        else
                        {
                            sb.Append("<td>");
                        }

                        sb.AppendFormat(
                            "<input type='text' tabIndex ='{0}' maxlength='2' class='hourInput hour' value='{1}' />:<input type='text' tabIndex ='{0}' maxlength='2' class='minuteInput minute' value='{2}' />",
                            i,
                            (workDetail.work_hour == "00" && workDetail.work_minute == "00") ? "" : workDetail.work_hour,
                            (workDetail.work_minute == "00" && workDetail.work_hour == "00") ? "" : workDetail.work_minute);
                        sb.AppendFormat(
                            "<input type='hidden' class='timeValue'  value='{0}' isChanged = '0'/>",
                            workDetail.actual_work_time);
                        sb.Append("</td>");
                        i++;
                    }
                    sb.Append("</tr>");
                    k++;
                }
                return sb.ToString();
            }
        }


        /// <summary>
        /// Generate the html content to increase the rendering speed(work_start_time)
        /// </summary>
        public string HtmlContentWorkStartTime
        {
            get
            {
                var sb = new StringBuilder();
                var sb1 = new StringBuilder();
                var sb2 = new StringBuilder();
                var sb3 = new StringBuilder();
                var sb4 = new StringBuilder();
                var sb5 = new StringBuilder();
                int i = 32;
                int date = 1;
                foreach (var attendentWorkDetail in WorkAttendanceDetail)
                {
                    DateTime d = new DateTime((int)target_year, (int)target_month, date);
                    string tdTag = string.Empty;
                    if ((HolidayInfo.special_holiday.Contains(d) || HolidayInfo.weekly_holiday.Contains(d.DayOfWeek))
                            && d.DayOfWeek == DayOfWeek.Saturday)
                    {
                        tdTag = "<td class=\"holiday saturday\">";
                    }
                    else if ((HolidayInfo.special_holiday.Contains(d) || HolidayInfo.weekly_holiday.Contains(d.DayOfWeek))
                        && d.DayOfWeek == DayOfWeek.Sunday)
                    {
                        tdTag = "<td class=\"holiday sunday\">";
                    }
                    else if (HolidayInfo.special_holiday.Contains(d) || HolidayInfo.weekly_holiday.Contains(d.DayOfWeek))
                    {
                        tdTag = "<td class=\"holiday\">";
                    }
                    else if (d.DayOfWeek == DayOfWeek.Saturday)
                    {
                        tdTag = "<td class=\"saturday\">";
                    }
                    else if (d.DayOfWeek == DayOfWeek.Sunday)
                    {
                        tdTag = "<td class=\"sunday\">";
                    }
                    else
                    {
                        tdTag = "<td>";
                    }

                    sb1.Append(tdTag);
                    sb1.AppendFormat(
                        "<input type='text' tabIndex ='{0}' maxlength='2' class='start_time_hour hour' value='{1}' />:<input type='text' tabIndex ='{0}' maxlength='2' class='start_time_minute minute' value='{2}' />",
                        i,
                        (attendentWorkDetail.work_start_time_hour == "" && attendentWorkDetail.work_start_time_minute == "") ? "" : attendentWorkDetail.work_start_time_hour,
                        (attendentWorkDetail.work_start_time_minute == "" && attendentWorkDetail.work_start_time_hour == "") ? "" : attendentWorkDetail.work_start_time_minute);
                    sb1.AppendFormat(
                        "<input type='hidden' class='start_time'  value='{0}' isChanged='0'/>",
                        attendentWorkDetail.work_start_time);
                    sb1.Append("</td>");

                    sb2.Append(tdTag);
                    sb2.AppendFormat(
                        "<input type='text' tabIndex ='{0}' maxlength='2' class='end_time_hour hour' value='{1}' />:<input type='text' tabIndex ='{0}' maxlength='2' class='end_time_minute minute' value='{2}' />",
                        i,
                        (attendentWorkDetail.work_end_time_hour == "" && attendentWorkDetail.work_end_time_minute == "") ? "" : attendentWorkDetail.work_end_time_hour,
                        (attendentWorkDetail.work_end_time_minute == ""&& attendentWorkDetail.work_end_time_hour == "") ? "" : attendentWorkDetail.work_end_time_minute);
                    sb2.AppendFormat(
                        "<input type='hidden' class='end_time'  value='{0}' isChanged='0'/>",
                        attendentWorkDetail.work_end_time);
                    sb2.Append("</td>");

                    sb3.Append(tdTag);
                    sb3.AppendFormat(
                        "<input type='text' tabIndex ='{0}' maxlength='2' class='rest_time_hour hour' value='{1}' />:<input type='text' tabIndex ='{0}' maxlength='2' class='rest_time_minute minute' value='{2}' />",
                        i,
                        (attendentWorkDetail.rest_time_hour == "" && attendentWorkDetail.rest_time_minute == "") ? "" : attendentWorkDetail.rest_time_hour,
                        (attendentWorkDetail.rest_time_minute == "" && attendentWorkDetail.rest_time_hour == "") ? "" : attendentWorkDetail.rest_time_minute);
                    sb3.AppendFormat(
                        "<input type='hidden' class='rest_time'  value='{0}' isChanged='0'/>",
                        attendentWorkDetail.rest_time);
                    sb3.Append("</td>");

                    sb4.Append(tdTag);
                    sb4.AppendFormat(
                        "<input type='text' tabIndex ='{0}' maxlength='2' class='allowed_cost_time_hour hour' value='{1}' />:<input type='text' tabIndex ='{0}' maxlength='2' class='allowed_cost_time_minute minute' value='{2}' />",
                        i,
                        (attendentWorkDetail.allowed_cost_time_hour == "" && attendentWorkDetail.allowed_cost_time_minute == "") ? "" : attendentWorkDetail.allowed_cost_time_hour,
                        (attendentWorkDetail.allowed_cost_time_minute == "" && attendentWorkDetail.allowed_cost_time_hour == "") ? "" : attendentWorkDetail.allowed_cost_time_minute);

                    sb4.AppendFormat(
                        "<input type='hidden' class='allowed_cost_time'  value='{0}' isChanged='0'/>",
                        attendentWorkDetail.allowed_cost_time);
                    sb4.Append("</td>");

                    sb5.Append(tdTag);
                    sb5.AppendFormat("<select tabIndex ='{0}' class='col_2 col_2_textbox column' name='attendance_type'>",  i);

                    // Add flag check acttendace_type_id exists in AttendanceTypeList
                    var hasSelected = false;

                    foreach (var attendanceType in AttendanceTypeList) {
                        if (attendanceType.Value == attendentWorkDetail.attendance_type_id.ToString())
                        {
                            sb5.AppendFormat("<option value='" + attendanceType.Value + "' selected>" + attendanceType.Text + "</option>");
                            hasSelected = true;
                        }
                        else
                        {
                            sb5.AppendFormat("<option value='" + attendanceType.Value + "'>" + attendanceType.Text + "</option>");
                        }
                    }


                    if (!hasSelected && attendentWorkDetail.attendance_type_id != 0)
                    {
                        sb5.AppendFormat("<option value='" + attendentWorkDetail.attendance_type_id + "' selected visibled hidden>" + attendentWorkDetail.attendance_type_name + "</option>");
                    }

                    sb5.AppendFormat("</select>");
                    sb5.AppendFormat(
                    "<input type='hidden' class='attendance_type' value='{0}' isChanged='0'/>",
                    attendentWorkDetail.attendance_type_id.ToString());
                    sb5.Append("</td>");
                    i++;
                    date++;
                }

                sb.Append("<tr class='tr-start-time old'>");
                sb.Append(sb1.ToString());
                sb.Append("</tr>");
                sb.Append("<tr class='tr-end-time even'>");
                sb.Append(sb2.ToString());
                sb.Append("</tr>");
                sb.Append("<tr class='tr-rest-time old'>");
                sb.Append(sb3.ToString());
                sb.Append("</tr>");
                sb.Append("<tr class='tr-allowed-cost-time even'>");
                sb.Append(sb4.ToString());
                sb.Append("</tr>");
                sb.Append("<tr class='tr-attend-type even'>");
                sb.Append(sb5.ToString());
                sb.Append("</tr>");

                return sb.ToString();
            }
        }
        /// <summary>
        /// Generate the html content to increase the rendering speed(work_start_time)
        /// </summary>
        public string HtmlCheckbox
        {
            get
            {
                var sb = new StringBuilder();
                if (ActualWorkList.Count == 0)
                {
                    sb.AppendFormat(
                   "<input type='hidden' name='actualWorkReadonly'  value='{0}' />",
                   ActualWorkList.Count.ToString());
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Return the number of days there are in a month
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfDays()
        {
            if (target_month.HasValue && target_year.HasValue)
            {
                if (target_month.Value == 1 || target_month.Value == 3 || target_month.Value == 5
                    || target_month.Value == 7 || target_month.Value == 8 || target_month.Value == 10
                    || target_month.Value == 12) return 31;
                else if (target_month.Value == 4 || target_month.Value == 6 || target_month.Value == 9
                         || target_month.Value == 11) return 30;
                else if (target_year.Value % 4 == 0 && target_year.Value % 100 != 0 || target_year.Value % 400 == 0) return 29;
                else
                {
                    return 28;
                }

            }
            return 0;
        }
    }
}