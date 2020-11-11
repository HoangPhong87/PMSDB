using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Models.PMS06002;

namespace ProjectManagementSystem.ViewModels.PMS06002
{
    public class PMS06002DetailNewViewModel
    {
        /// <summary>
        /// Search condition of the displayed list
        /// </summary>
        public DetailCondition Condition { get; set; }

        [DisplayName("")]
        public string GroupName { get; set; }

        [DisplayName("")]
        public string UserName { get; set; }

        [DisplayName("")]
        public string EstimatedTime { get; set; }

        [DisplayName("")]
        public string ActualTime { get; set; }

        [DisplayName("")]
        public string EstimatedTimeTotal { get; set; }

        [DisplayName("")]
        public string TotalCost { get; set; }
        
        [DisplayName("")]
        public string TotalIncome { get; set; }

        [DisplayName("")]
        public string RegistType { get; set; }

        [DisplayName("")]
        public string EmployeeNo { get; set; }

        public UpdateInfo UpdateInfo { get; set; }
        /// <summary>
        /// Text label to display the current range of months
        /// </summary>
        [DisplayName("")]
        public string CurrentYearMonth { get; set; }

        public List<IDictionary<string, object>> ActualWorkDetailNew { get; set; }

        public IList<UserActualWorkDetailPlus> UserActualWorkDetailPlus { get; set; }

        public HolidayInfo HolidayInfo { get; set; }

        public bool CbRegistType { get; set; }
        public PMS06002DetailNewViewModel()
        {
            Condition = new DetailCondition();
            GroupName = " ";
            UserName = " ";
            EstimatedTime = " ";
            ActualTime = " ";
            EstimatedTimeTotal = " ";
            TotalCost = " ";
            TotalIncome = " ";
            UpdateInfo = new UpdateInfo();
            CbRegistType = true;
			EmployeeNo = " ";
        }
    }
}
