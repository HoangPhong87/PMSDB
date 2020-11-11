using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS06002
{
    public class ActualWorkDetailNew
    {
        public decimal work_start_time { get; set; }

        public decimal work_end_time { get; set; }

        public decimal rest_time { get; set; }

        public int attendance_type_id { get; set; }

        public decimal? actual_work_time { get; set; }
    }
}