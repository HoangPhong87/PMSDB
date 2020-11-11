#region License
/// <copyright file="BranchListExportCSV.cs" company="i-Enter Asia">
/// Copyright © 2017 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>DTQ - Clone</author>
/// <createdDate>2017/06/29</createdDate>
#endregion

namespace ProjectManagementSystem.Models.PMS11001
{
    /// <summary>
    /// BranchListExportCSV model class
    /// </summary>
    public class BranchListExportCSV
    {
        public string peta_rn { get; set; }

        public string location_name { get; set; }

        public string display_name { get; set; }

        public string remarks { get; set; }

        public string upd_date { get; set; }

        public string upd_user { get; set; }
    }
}