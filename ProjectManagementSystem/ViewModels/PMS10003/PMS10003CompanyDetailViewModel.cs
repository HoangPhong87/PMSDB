using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10003;

namespace ProjectManagementSystem.ViewModels.PMS10003
{
    public class Weekday
    {
        public bool isHoliday { get; set; }
        public Weekday(bool _isHoliday)
        {
            isHoliday = _isHoliday;
        }
        public Weekday()
        {
            isHoliday = false;
        }
    }

    public class WeekPointDay
    {
        public bool isCheckPointWeek { get; set; }
        public WeekPointDay(bool _isCheckPointWeek)
        {
            isCheckPointWeek = _isCheckPointWeek;
        }
        public WeekPointDay()
        {
            isCheckPointWeek = false;
        }
    }

    public class PMS10003CompanyDetailViewModel
    {
        public CompanySettingPlus COMPANY_SETTING { get; set; }
        public List<Weekday> HOLIDAY_TYPE_LIST { get; set; }
        public List<WeekPointDay> CHECK_POINT_WEEK_LIST { get; set; }
        public List<SelectListItem> DEFAULT_WORK_TIME_LIST { get; set; }
        public List<SelectListItem> PASSWORD_INPUT_LIMIT_LIST { get; set; }
        public List<SelectListItem> PASSWORD_EFFECTIVE_TIME_LIST { get; set; }
        public List<SelectListItem> CACULATE_TYPE_LIST { get; set; }
        public List<SelectListItem> WORK_TIME_UNIT_LIST { get; set; }
        public IList<Holiday> SPECIAL_HOLIDAY_LIST { get; set; }
        public PMS10003CompanyDetailViewModel()
        {
            COMPANY_SETTING = new CompanySettingPlus();
            HOLIDAY_TYPE_LIST = new List<Weekday>();
            CHECK_POINT_WEEK_LIST = new List<WeekPointDay>();
            DEFAULT_WORK_TIME_LIST = new List<SelectListItem>()
            {
                new SelectListItem { Text = "1", Value = "1" },
                new SelectListItem { Text = "2", Value = "2" },
                new SelectListItem { Text = "3", Value = "3" },
                new SelectListItem { Text = "4", Value = "4" },
                new SelectListItem { Text = "5", Value = "5" },
                new SelectListItem { Text = "6", Value = "6" },
                new SelectListItem { Text = "7", Value = "7" },
                new SelectListItem { Text = "8", Value = "8" },
                new SelectListItem { Text = "9", Value = "9" },
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "11", Value = "11" },
                new SelectListItem { Text = "12", Value = "12" },
                new SelectListItem { Text = "13", Value = "13" },
                new SelectListItem { Text = "14", Value = "14" },
                new SelectListItem { Text = "15", Value = "15" },
                new SelectListItem { Text = "16", Value = "16" },
                new SelectListItem { Text = "17", Value = "17" },
                new SelectListItem { Text = "18", Value = "18" },
                new SelectListItem { Text = "19", Value = "19" },
                new SelectListItem { Text = "20", Value = "20" },
                new SelectListItem { Text = "21", Value = "21" },
                new SelectListItem { Text = "22", Value = "22" },
                new SelectListItem { Text = "23", Value = "23" },
                new SelectListItem { Text = "24", Value = "24" }
            };
            PASSWORD_INPUT_LIMIT_LIST = new List<SelectListItem>()
            {
                new SelectListItem { Text = "1", Value = "1" },
                new SelectListItem { Text = "2", Value = "2" },
                new SelectListItem { Text = "3", Value = "3" },
                new SelectListItem { Text = "4", Value = "4" },
                new SelectListItem { Text = "5", Value = "5" },
                new SelectListItem { Text = "6", Value = "6" },
                new SelectListItem { Text = "7", Value = "7" },
                new SelectListItem { Text = "8", Value = "8" },
                new SelectListItem { Text = "9", Value = "9" },
                new SelectListItem { Text = "10", Value = "10" }
            };
            PASSWORD_EFFECTIVE_TIME_LIST = new List<SelectListItem>()
            {
                new SelectListItem { Text = "0", Value = "0" },
                new SelectListItem { Text = "1", Value = "1" },
                new SelectListItem { Text = "2", Value = "2" },
                new SelectListItem { Text = "3", Value = "3" },
                new SelectListItem { Text = "4", Value = "4" },
                new SelectListItem { Text = "5", Value = "5" },
                new SelectListItem { Text = "6", Value = "6" },
                new SelectListItem { Text = "7", Value = "7" },
                new SelectListItem { Text = "8", Value = "8" },
                new SelectListItem { Text = "9", Value = "9" },
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "11", Value = "11" },
                new SelectListItem { Text = "12", Value = "12" }
            };
            CACULATE_TYPE_LIST = new List<SelectListItem>()
            {
                new SelectListItem { Value = "01", Text = "小数点以下切り捨て" },
                new SelectListItem { Value = "02", Text = "小数点第一位切り上げ" },
                new SelectListItem { Value = "03", Text = "小数点第一位四捨五入" }
            };
            WORK_TIME_UNIT_LIST = new List<SelectListItem>()
            {
                new SelectListItem { Text = "5", Value = "5" },
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "15", Value = "15" },
                new SelectListItem { Text = "20", Value = "20" },
                new SelectListItem { Text = "30", Value = "30" },
                new SelectListItem { Text = "60", Value = "60" }
            };
            SPECIAL_HOLIDAY_LIST = new List<Holiday>();
        }
    }
}