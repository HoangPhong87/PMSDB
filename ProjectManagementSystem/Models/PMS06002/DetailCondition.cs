namespace ProjectManagementSystem.Models.PMS06002
{
    using System;
    using ProjectManagementSystem.Common;

    /// <summary>
    /// Containing fields used as search conditions for ActualWorkDetail page
    /// </summary>
    [Serializable]
    public class DetailCondition
    {
        public DetailCondition()
        {
            WorkTimeUnit = Constant.TimeUnit.DAY;
            SelectedMonth = Utility.GetCurrentDateTime().Month;
            SelectedYear = Utility.GetCurrentDateTime().Year;
            UserId = "";
        }

        public string UserId { get; set; }

        public string WorkTimeUnit { get; set; }

        public int SelectedMonth { get; set; }

        public int SelectedYear { get; set; }
        
    }
}