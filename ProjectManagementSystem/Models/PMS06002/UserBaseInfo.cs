using System;

namespace ProjectManagementSystem.Models.PMS06002
{
    public class UserBaseInfo
    {
        public string company_code { get; set; }
        public string group_name { get; set; }
        public string display_name { get; set; }
        public string employee_no { get; set; }
        public string user_name_sei { get; set; }
        public string user_name_mei { get; set; }
        public string location_name { get; set; }
        public DateTime? entry_date { get; set; }
    }
}