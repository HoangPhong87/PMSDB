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
using ProjectManagementSystem.Models.Entities;

    /// <summary>
    /// ModelView class for the ActualWorkRegist screen
    /// </summary>
    public class PMS06002ActualWorkRegistModelNew
    {
        public int? target_date { get; set; }
        public int? target_month { get; set; }

        public int? target_year { get; set; }

        public int? user_sys_id { get; set; }

        public string company_code { get; set; }

        public string registerType { get; set; }
        /// <summary>
        /// List of Attendance
        /// </summary>
        public IList<SelectListItem> AttendanceTypeList { get; set; }

        public AttendanceRecord AttendanceRecordInfor { get; set; }

        public IList<MemberActualWorkListPlus> MemberActualWorkList { get; set; }

        public string work_start_time_hour
        {
            get
            {
                if (AttendanceRecordInfor.work_start_time.HasValue)
                {
                    var hour = Math.Floor(AttendanceRecordInfor.work_start_time.Value);
                    return string.Format("{0:00}", hour);
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Return the minute value in 2 digits
        /// </summary>
        public string work_start_time_minute
        {
            get
            {
                if (AttendanceRecordInfor.work_start_time.HasValue)
                {
                    var min = Math.Round(60 * (AttendanceRecordInfor.work_start_time.Value - Math.Floor(AttendanceRecordInfor.work_start_time.Value)));
                    return string.Format("{0:00}", min);
                }
                else
                    return "";
            }
        }


        /// <summary>
        /// Return the hour value in two digits
        /// </summary>
        public string work_end_time_hour
        {
            get
            {
                if (AttendanceRecordInfor.work_end_time.HasValue)
                {
                    var hour = Math.Floor(AttendanceRecordInfor.work_end_time.Value);
                    return string.Format("{0:00}", hour);
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Return the minute value in 2 digits
        /// </summary>
        public string work_end_time_minute
        {
            get
            {
                if (AttendanceRecordInfor.work_end_time.HasValue)
                {
                    var min = Math.Round(60 * (AttendanceRecordInfor.work_end_time.Value - Math.Floor(AttendanceRecordInfor.work_end_time.Value)));
                    return string.Format("{0:00}", min);
                }
                else
                    return "";
            }
        }


        /// <summary>
        /// Return the hour value in two digits
        /// </summary>
        public string rest_time_hour
        {
            get
            {
                if (AttendanceRecordInfor.rest_time.HasValue)
                {
                    var hour = Math.Floor(AttendanceRecordInfor.rest_time.Value);
                    return string.Format("{0:00}", hour);
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Return the minute value in 2 digits
        /// </summary>
        public string rest_time_minute
        {
            get
            {
                if (AttendanceRecordInfor.rest_time.HasValue)
                {
                    var min = Math.Round(60 * (AttendanceRecordInfor.rest_time.Value - Math.Floor(AttendanceRecordInfor.rest_time.Value)));
                    return string.Format("{0:00}", min);
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Return the hour value in two digits
        /// </summary>
        public string allowed_cost_time_hour
        {
            get
            {
                if (AttendanceRecordInfor.allowed_cost_time.HasValue)
                {
                    var hour = Math.Floor(AttendanceRecordInfor.allowed_cost_time.Value);
                    return string.Format("{0:00}", hour);
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Return the minute value in 2 digits
        /// </summary>
        public string allowed_cost_time_minute
        {
            get
            {
                if (AttendanceRecordInfor.allowed_cost_time.HasValue)
                {
                    var min = Math.Round(60 * (AttendanceRecordInfor.allowed_cost_time.Value - Math.Floor(AttendanceRecordInfor.allowed_cost_time.Value)));
                    return string.Format("{0:00}", min);
                }
                else
                {
                    return "";
                }
            }
        }
    }
}