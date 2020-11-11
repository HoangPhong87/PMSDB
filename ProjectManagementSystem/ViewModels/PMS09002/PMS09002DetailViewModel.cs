#region License
/// <copyright file="PMS09002DetailViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/15</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS09002
{
    using ProjectManagementSystem.Models.PMS09002;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class PMS09002DetailViewModel
    {
        public int UserID { get; set; }

        public int TargetYear { get; set; }

        public int TargetMonth { get; set; }

        public string UserName { get; set; }
        public string ContractTypeName { get; set; }
        public string LocationName { get; set; }
        public string ContractTypeID { get; set; }
        public PMS09002DetailViewModel()
        {
        }
    }
}
