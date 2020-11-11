using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS11003
{
    [Serializable]
    public class Condition
    {
        public Condition()
        {
            WorkTimeUnit = Constant.TimeUnit.DAY;
            var d = Utility.GetCurrentDateTime();
            EndMonth = new DateTime(d.Year, d.Month, 15, 0, 0, 0);
            StartMonth = EndMonth.AddMonths(-5);
        }

        public DateTime StartMonth { get; set; }

        public DateTime EndMonth { get; set; }

        public string CompanyCode { get; set; }

        public string GroupId { get; set; }

        public string ContractTypeId { get; set; }

        public string BranchId { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public List<ContractType> List_Contract { get; set; }
        
		public List<Group> List_Group { get; set; }
        
		public List<string> List_Month { get; set; }
        
		public string WorkTimeUnit { get; set; }
    }

    public class TimeListBudget
    {
        public int target_year { set; get; }

        public string target_month { set; get; }
    }
}