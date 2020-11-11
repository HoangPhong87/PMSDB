#region License
/// <copyright file="PMS09001ListViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/09/04</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS09001
{
    using ProjectManagementSystem.Models.PMS09001;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class PMS09001SalesGroupDetailViewModel
    {
        public PMS09001SalesGroupDetailViewModel()
        {
            GroupName = "";
            TotalSales = "";
            TotalCost = "";
            TotalProfit = "";
            Condition = new SalesGroupDetailCondition();
        }
        public SalesGroupDetailCondition Condition { get; set; }
        public string GroupName { get; set; }
        public string TotalSales { get; set; }
        public string TotalCost { get; set; }
        public string TotalProfit { get; set; }
        public string CurrentYearMonth { get; set; }
        public string ContractTypeName { get; set; }
        public string LocationName { get; set; }
        public string ContractTypeID { get; set; }
    }
}