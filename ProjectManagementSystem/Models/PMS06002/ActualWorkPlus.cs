#region Licence
// //--------------------------------------------------------------------------------------------------------------------
// //<copyright file="ActualWorkPlus.cs" company="i-Enter Asia">
// // Copyright © 2014 i-Enter Asia. All rights reserved.
// //</copyright>
// //<project>Project Management System</project>
// //<author>Nguyen Minh Hien</author>
// //<email>hiennm@live.com</email>
// //<createdDate>27-05-2014</createdDate>
// //<summary>
// // TODO: Update summary.
// //</summary>
// //--------------------------------------------------------------------------------------------------------------------
#endregion
namespace ProjectManagementSystem.Models.PMS06002
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represent the actual work detail on a project phase of an user in a month
    /// </summary>
    public class ActualWorkPlus
    {
        public string company_code { get; set; }

        public int? project_sys_id { get; set; }

        public string project_no { get; set; }

        public string project_name { get; set; }

        public int? phase_id { get; set; }

        public string phase_name { get; set; }

        public int? user_sys_id { get; set; }

        public int? target_month { get; set; }

        public int? target_year { get; set; }

        public decimal work_start_time { get; set; }

        public decimal work_end_time { get; set; }

        public decimal rest_time { get; set; }

        public int attendance_type_id { get; set; }

        public string regist_type { get; set; }

        /// <summary>
        /// A list of actuak work on each day of month
        /// </summary>
        public IList<WorkDetailPlus> workDetails { get; set; }
        
    }
}