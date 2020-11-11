using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Models.PMS06002;

namespace ProjectManagementSystem.ViewModels.PMS06002
{
    public class PMS06002DetailViewModel
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

        public bool CbRegistType { get; set; }
        public PMS06002DetailViewModel()
        {
            Condition = new DetailCondition();
            GroupName = " ";
            UserName = " ";
            EstimatedTime = " ";
            ActualTime = " ";
            EstimatedTimeTotal = " ";
            TotalCost = " ";
            TotalIncome = " ";
            CbRegistType = true;
            UpdateInfo = new UpdateInfo();
        }
    }
}
