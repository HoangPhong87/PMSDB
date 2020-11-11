#region License
/// <copyright file="ConsumtionTaxListExportCSV.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/23</createdDate>
#endregion

namespace ProjectManagementSystem.Models.PMS07001
{
    /// <summary>
    /// ConsumtionTaxListExportCSV model class
    /// </summary>
    public class ConsumptionTaxListExportCSV
    {
        public string peta_rn { get; set; }

        public string apply_start_date { get; set; }

        public string tax_rate { get; set; }

        public string remarks { get; set; }

        public string ins_date { get; set; }

        public string ins_user { get; set; }
    }
}