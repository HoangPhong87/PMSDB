using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS06002
{
    public class HolidayInfo
    {
        public IList<DayOfWeek> weekly_holiday { get; set; }
        public IList<DateTime> special_holiday { get; set; }
    }
}