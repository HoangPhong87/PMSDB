#region Licence
// //--------------------------------------------------------------------------------------------------------------------
// //<copyright file="Condition.cs" company="i-Enter Asia">
// // Copyright © 2014 i-Enter Asia. All rights reserved.
// //</copyright>
// //<project>Project Management System</project>
// //<author>Nguyen Minh Hien</author>
// //<email>hiennm@live.com</email>
// //<createdDate>27-05-2014</createdDate>
// //<summary>
// // Condition class for PMS06002 screen
// //</summary>
// //--------------------------------------------------------------------------------------------------------------------
#endregion

namespace ProjectManagementSystem.Models.PMS06002
{
    using System;
    using System.Collections.Generic;
    using ProjectManagementSystem.Common;

    /// <summary>
    /// Containing fields used as search conditions for ActualWorkList page
    /// </summary>
    [Serializable]
    public class Condition
    {
        public Condition()
        {
            WorkTimeUnit = Constant.TimeUnit.DAY;
            var d = Utility.GetCurrentDateTime();
            EndMonth = new DateTime(d.Year, d.Month,15,0,0,0);
            StartMonth = EndMonth.AddMonths(-5);
        }

        public string CompanyCode { get; set; }

        public int? GroupId { get; set; }

        public string DisplayName { get; set; }

        public string WorkTimeUnit {get; set; }

        public DateTime StartMonth { get; set; }

        public DateTime EndMonth { get; set; }

        public bool DELETED_INCLUDE { get; set; }

        public bool RETIREMENT_INCLUDE { get; set; }

        public string LOCATION_ID { get; set; }

        public int Filterable { get; set; }

        // Add new 3 items for scren ActualListByIndividualPhase
        public string PROJECT_NAME { get; set; }

        public string CONTRACT_TYPE_ID { get; set; }

        public string PHASE_ID { get; set; }

        public string PHASE_BY_CONTRACT{ get; set; }
    }
}