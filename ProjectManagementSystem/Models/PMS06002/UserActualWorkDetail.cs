using System;

namespace ProjectManagementSystem.Models.PMS06002
{
    [Serializable]
    public class UserActualWorkDetail
    {
        public string project_no { get; set; }
        public string project_name { get; set; }
        public string rank { get; set; }
        public decimal progress { get; set; }
        public decimal plan_effort { get; set; }
        public decimal actual_effort { get; set; }
        public decimal personal_profit_rate { get; set; }
        public decimal project_actual_profit { get; set; }
    }
}