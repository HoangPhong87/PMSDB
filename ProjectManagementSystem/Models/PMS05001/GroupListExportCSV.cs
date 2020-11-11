#region License
/// <copyright file="GroupListExportCSV.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/22</createdDate>
#endregion

namespace ProjectManagementSystem.Models.PMS05001
{
    /// <summary>
    /// GroupListExportCSV model class
    /// </summary>
    public class GroupListExportCSV
    {
        public string peta_rn { get; set; }

        public string group_name { get; set; }

        public string display_name { get; set; }

        public string budget_setting_flg { get; set; }

        public string remarks { get; set; }

        public string upd_date { get; set; }

        public string upd_user { get; set; }
    }
}