namespace ProjectManagementSystem.Models.PMS06002
{
    public class WorkDetailInfo
    {
        public decimal plan_effort { get; set; }

        public decimal estimate_cost { get; set; }

        public decimal actual_effort { get; set; }

        public decimal actual_cost { get; set; }

        public string regist_type {get; set; }
    }
}