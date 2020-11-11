using ProjectManagementSystem.Models.Entities;
using System;

namespace ProjectManagementSystem.Models.PMS01001
{
    /// <summary>
    /// m_user plus model class
    /// </summary>
    public class UserPlus : User
    {
        public string company_logo_img_path { get; set; }
        public string decimal_calculation_type { get; set; }
        public int working_time_unit_minute { get; set; }
        public int is_expired_password { get; set; }
        public string company_name { get; set; }
        public int data_editable_time { get; set; }
        public string group_name { get; set; }
    }
}