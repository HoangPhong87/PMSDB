#region License
/// <copyright file="PhaseListExportCSV.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/21</createdDate>
#endregion

namespace ProjectManagementSystem.Models.PMS04001
{
    /// <summary>
    /// PhaseListExportCSV model class
    /// </summary>
    public class PhaseListExportCSV
    {
        public string peta_rn { get; set; }

        public string phase_name { get; set; }

        public string display_name { get; set; }

        public string remarks { get; set; }

        public string estimate_target { get; set; }

        public string upd_date { get; set; }

        public string upd_user { get; set; }
    }
}