using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS06002
{
    public class AttendanceInfo
    {
        public IList<AttendanceDetail> AttDetail { get; set; }
        public UserBaseInfo UserInfo { get; set; }
        public int WorkClosingDay { get; set; }
        public int SelectedYear { get; set; }
        public int SelectedMonth { get; set; }
    }
}