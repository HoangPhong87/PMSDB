using System;

namespace ProjectManagementSystem.Models.PMS06002
{
    [Serializable]
    public class UserActualWorkDetailPlus : UserActualWorkDetail
    {
        public int project_sys_id { get; set; }
        public string company_code { get; set; }
        public int del_flg { get; set; }
        public int sales_type { get; set; }
        public string status { get; set; }
        public int count_project_plan { get; set; }
    }
}